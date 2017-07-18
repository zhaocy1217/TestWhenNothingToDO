using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameSerializer
{
    public const string DOM_ATTR_DISABLE = "DIS";
    public const string DOM_ATTR_IS_NULL = "NULL";
    private const string DOM_ATTR_JUDGETYPE_ARRAY = "Arr";
    private const string DOM_ATTR_JUDGETYPE_COMMON = "Com";
    private const string DOM_ATTR_JUDGETYPE_CUSTOM = "Cus";
    private const string DOM_ATTR_JUDGETYPE_ENUM = "Enum";
    private const string DOM_ATTR_JUDGETYPE_PRIMITIVE = "Pri";
    private const string DOM_ATTR_JUDGETYPE_REF = "Ref";
    public const string DOM_ATTR_LIGHTMAP_IDX = "I";
    public const string DOM_ATTR_LIGHTMAP_TILEOFFSET = "TO";
    public const string DOM_ATTR_NAME_ARRAY_SIZE = "Size";
    public const string DOM_ATTR_NAME_JUDGETYPE = "JT";
    public const string DOM_ATTR_NAME_OBJECT_TYPE = "Type";
    public const string DOM_ATTR_NAME_PREFAB = "PFB";
    public const string DOM_ATTR_NAME_TRANSFORM_ACTIVE = "A";
    public const string DOM_ATTR_NAME_TRANSFORM_LAYER = "L";
    public const string DOM_ATTR_NAME_TRANSFORM_LOC_POS = "P";
    public const string DOM_ATTR_NAME_TRANSFORM_LOC_ROT = "R";
    public const string DOM_ATTR_NAME_TRANSFORM_LOC_SCL = "S";
    public const string DOM_ATTR_NAME_TRANSFORM_TAG = "Tag";
    public const string DOM_ATTR_NAME_VALUE = "V";
    public const string DOM_LIGHTMAP_INFO = "LMI";
    public const string DOM_NODE_NAME_CHILDNODE = "CHD";
    public const string DOM_NODE_NAME_COMPONENT = "Cop";
    public const string DOM_NODE_NAME_OBJNAME = "ON";
    public const string DOM_NODE_NAME_TRANSFORM = "T";
    public const string DOM_ROOT_TAG = "root";
    public const string K_EXPORT_PATH = "/Resources/SceneExport";
    public const string K_EXPORT_PATH_IN_RESOURCES_FOLDER = "SceneExport";
    public string m_domPath = Application.get_dataPath();
    private ArrayList m_storedObjs = new ArrayList();
    public const string PREFABREF_ASSET_DIR = "PrefabAssets";
    private const string RESOURCES_DIR = "Assets/Resources";
    private const BindingFlags s_bindFlag = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly);
    private static Type[] s_ComponentsCannotSerialize = new Type[] { typeof(Rigidbody), typeof(Rigidbody2D), typeof(MeshFilter), typeof(Renderer), typeof(PhysicMaterial), typeof(Avatar), typeof(Material), typeof(Animator), typeof(MeshCollider) };
    private static DictionaryView<Type, ICustomizedComponentSerializer> s_componentSerializerTypeCache = null;
    private static Type[] s_FieldsCannotSerialize = new Type[] { typeof(Mesh) };
    private static GameObject s_gameObjectRoot4Read = null;
    private static DictionaryView<Type, ICustomizedObjectSerializer> s_objectSerializerTypeCache = null;
    private static string s_saveFailStr = string.Empty;
    private static int s_saveRecurTimes = 0;
    private static Dictionary<string, Type> s_typeCache = new Dictionary<string, Type>();

    private static bool CheckObjectLegal(GameObject go, Component[] cpnt, GameObject prefab)
    {
        if (prefab == null)
        {
            for (int i = 0; i < cpnt.Length; i++)
            {
                for (int j = 0; j < s_ComponentsCannotSerialize.Length; j++)
                {
                    if ((cpnt[i] != null) && (cpnt[i].GetType() == s_ComponentsCannotSerialize[j]))
                    {
                        Debug.LogWarning("忽略保存对象:" + GetObjectHierachy(go) + "，因为其有" + GetPureType(s_ComponentsCannotSerialize[j].ToString()) + "组件却不是Prefab");
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public static object CreateInstance(string typeStr)
    {
        return CreateInstance(GetType(typeStr));
    }

    public static object CreateInstance(Type type)
    {
        object obj2 = null;
        try
        {
            obj2 = Activator.CreateInstance(type);
        }
        catch (Exception exception)
        {
            object[] inParameters = new object[] { (type == null) ? "UnkownType" : type.ToString(), exception };
            DebugHelper.Assert(obj2 != null, "{0} create failed. due to exception: {1}", inParameters);
        }
        return obj2;
    }

    private static void DeserializeObject(BinaryNode objInfoNode, GameObject go)
    {
        BinaryNode node = objInfoNode.SelectSingleNode("T");
        if (node != null)
        {
            try
            {
                byte[] binaryAttribute = GetBinaryAttribute(node, "P");
                if (binaryAttribute != null)
                {
                    go.get_transform().set_position(UnityBasetypeSerializer.BytesToVector3(binaryAttribute));
                }
                byte[] data = GetBinaryAttribute(node, "R");
                if (data != null)
                {
                    go.get_transform().set_rotation(UnityBasetypeSerializer.BytesToQuaternion(data));
                }
                byte[] buffer3 = GetBinaryAttribute(node, "S");
                if (buffer3 != null)
                {
                    go.get_transform().set_localScale(UnityBasetypeSerializer.BytesToVector3(buffer3));
                }
                string attribute = GetAttribute(node, "L");
                if (attribute != null)
                {
                    go.set_layer(Convert.ToInt32(attribute));
                }
                string str2 = GetAttribute(node, "Tag");
                if (str2 != null)
                {
                    go.set_tag(str2);
                }
                string str3 = GetAttribute(node, "A");
                if (str3 != null)
                {
                    go.SetActive(str3.Equals("True"));
                }
            }
            catch (Exception)
            {
                object[] inParameters = new object[] { go.get_name() };
                DebugHelper.Assert(false, "Gameobject {0} transform load failed!", inParameters);
            }
        }
        BinaryNode node2 = objInfoNode.SelectSingleNode("LMI");
        if (node2 != null)
        {
            Renderer component = go.GetComponent<Renderer>();
            if (component != null)
            {
                string str4 = GetAttribute(node2, "I");
                if (str4 != null)
                {
                    component.set_lightmapIndex(Convert.ToInt32(str4));
                }
                else
                {
                    component.set_lightmapIndex(-1);
                }
                byte[] buffer4 = GetBinaryAttribute(node2, "TO");
                if (buffer4 != null)
                {
                    component.set_lightmapTilingOffset(UnityBasetypeSerializer.BytesToVector4(buffer4));
                }
            }
        }
    }

    public static GameObject FindRootGameObject(GameObject anyGo)
    {
        GameObject obj2 = anyGo;
        while (obj2.get_transform().get_parent() != null)
        {
            obj2 = obj2.get_transform().get_parent().get_gameObject();
        }
        return obj2;
    }

    public static string GetAttribute(BinaryNode node, string attName)
    {
        for (int i = 0; i < node.GetAttrNum(); i++)
        {
            BinaryAttr attr = node.GetAttr(i);
            if (attr.GetName() == attName)
            {
                return attr.GetValueString();
            }
        }
        return null;
    }

    public static byte[] GetBinaryAttribute(BinaryNode node, string attName)
    {
        for (int i = 0; i < node.GetAttrNum(); i++)
        {
            BinaryAttr attr = node.GetAttr(i);
            if (attr.GetName() == attName)
            {
                return attr.GetValue();
            }
        }
        return null;
    }

    private static ICustomizedComponentSerializer GetComponentSerlizer(Type type)
    {
        ICustomizedComponentSerializer serializer = null;
        if (componentSerializerTypeCache.TryGetValue(type, out serializer))
        {
            return serializer;
        }
        return null;
    }

    public static Object GetGameObjectFromPath(string pathname, string typeName = new string())
    {
        if (string.IsNullOrEmpty(pathname))
        {
            return null;
        }
        Object resource = null;
        Type type = typeof(GameObject);
        if (typeName != null)
        {
            type = GetType(typeName);
        }
        resource = (Object) GetResource(pathname, type);
        if (resource != null)
        {
            return resource;
        }
        if (s_gameObjectRoot4Read != null)
        {
            if (pathname == string.Format("/{0}", s_gameObjectRoot4Read.get_name()))
            {
                return s_gameObjectRoot4Read;
            }
            string str = pathname.Replace(string.Format("/{0}/", s_gameObjectRoot4Read.get_name()), string.Empty);
            Transform transform = s_gameObjectRoot4Read.get_transform().Find(str);
            if (transform == null)
            {
                return resource;
            }
            return transform.get_gameObject();
        }
        return GameObject.Find(pathname);
    }

    public static string GetGameObjectPathName(GameObject go)
    {
        string str = string.Empty;
        if (go == null)
        {
            return "null";
        }
        if ((str == null) || (str.Length == 0))
        {
            try
            {
                str = "/" + go.get_name();
                for (GameObject obj2 = go; obj2.get_transform().get_parent() != null; obj2 = obj2.get_transform().get_parent().get_gameObject())
                {
                    str = "/" + obj2.get_transform().get_parent().get_name() + str;
                }
            }
            catch (Exception exception)
            {
                Debug.Log("Get gameobject " + go.get_name() + " path failed!");
                Debug.LogError(exception);
                str = string.Empty;
            }
        }
        return str;
    }

    private static Type GetMIType(MemberInfo mi)
    {
        if (mi != null)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                {
                    FieldInfo info = (FieldInfo) mi;
                    return info.FieldType;
                }
                case MemberTypes.Property:
                {
                    PropertyInfo info2 = (PropertyInfo) mi;
                    return info2.PropertyType;
                }
            }
        }
        return null;
    }

    private static string GetMITypeStr(MemberInfo mi)
    {
        if (mi != null)
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                {
                    FieldInfo info = (FieldInfo) mi;
                    return info.FieldType.ToString();
                }
                case MemberTypes.Property:
                {
                    PropertyInfo info2 = (PropertyInfo) mi;
                    return info2.PropertyType.ToString();
                }
            }
        }
        return string.Empty;
    }

    private static object GetMIValue(MemberInfo mi, object owner)
    {
        try
        {
            PropertyInfo info2;
            if ((owner == null) || (mi == null))
            {
                goto Label_00C9;
            }
            MemberTypes memberType = mi.MemberType;
            if (memberType != MemberTypes.Field)
            {
                if (memberType == MemberTypes.Property)
                {
                    goto Label_003C;
                }
                goto Label_00C9;
            }
            FieldInfo info = (FieldInfo) mi;
            return info.GetValue(owner);
        Label_003C:
            info2 = (PropertyInfo) mi;
            return info2.GetValue(owner, null);
        }
        catch (Exception exception)
        {
            DebugHelper.Assert(mi != null);
            if (mi != null)
            {
                Debug.Log(string.Concat(new object[] { GetMITypeStr(mi), mi.MemberType, " ", mi.ReflectedType.ToString(), " ", mi.Name, " seems not support !!!", exception }));
            }
        }
    Label_00C9:
        return null;
    }

    public static string GetNodeAttr(BinaryNode node, string attrName)
    {
        for (int i = 0; i < node.GetAttrNum(); i++)
        {
            BinaryAttr attr = node.GetAttr(i);
            if (attr.GetName() == attrName)
            {
                return attr.GetValueString();
            }
        }
        return null;
    }

    public static object GetObject(BinaryNode currNode)
    {
        string nodeAttr = GetNodeAttr(currNode, "NULL");
        object o = null;
        if (nodeAttr == null)
        {
            string typeStr = GetNodeAttr(currNode, "Type");
            string s = GetNodeAttr(currNode, "V");
            string str4 = GetNodeAttr(currNode, "JT");
            if ("Arr".Equals(str4))
            {
                if (typeStr == null)
                {
                    return o;
                }
                Type elementType = GetType(typeStr.Replace("[]", string.Empty));
                if (elementType == null)
                {
                    Debug.LogError("Array type " + typeStr + " create failed!");
                    return null;
                }
                Array array = Array.CreateInstance(elementType, currNode.GetChildNum());
                for (int i = 0; i < array.Length; i++)
                {
                    array.SetValue(GetObject(currNode.GetChild(i)), i);
                }
                return array;
            }
            if ("Cus".Equals(str4))
            {
                if (typeStr != null)
                {
                    Type type = GetType(typeStr);
                    ICustomizedObjectSerializer objectSerlizer = GetObjectSerlizer(type);
                    if ((objectSerlizer != null) && (objectSerlizer is ICustomInstantiate))
                    {
                        o = ((ICustomInstantiate) objectSerlizer).Instantiate(currNode);
                    }
                    else
                    {
                        o = CreateInstance(type);
                    }
                    if (o == null)
                    {
                        return null;
                    }
                    if (objectSerlizer != null)
                    {
                        objectSerlizer.ObjectDeserialize(ref o, currNode);
                    }
                }
                return o;
            }
            if ("Enum".Equals(str4))
            {
                if (typeStr != null)
                {
                    o = Enum.ToObject(GetType(typeStr), int.Parse(s));
                }
                return o;
            }
            if ("Pri".Equals(str4))
            {
                if (typeStr != null)
                {
                    o = Convert.ChangeType(s, GetType(typeStr));
                }
                return o;
            }
            if ("Ref".Equals(str4))
            {
                Object gameObjectFromPath = GetGameObjectFromPath(s, typeStr);
                if (gameObjectFromPath != null)
                {
                    if (gameObjectFromPath is GameObject)
                    {
                        if (typeStr == null)
                        {
                            return o;
                        }
                        string pureType = GetPureType(typeStr);
                        if (!"GameObject".Equals(pureType))
                        {
                            o = (gameObjectFromPath as GameObject).GetComponent(pureType);
                            if (o == null)
                            {
                                Debug.LogError("No " + pureType + " component found in " + s);
                            }
                            return o;
                        }
                    }
                    return gameObjectFromPath;
                }
                o = null;
                Debug.LogError("Load gameobject " + s + " failed!");
                return o;
            }
            if ("Com".Equals(str4))
            {
                o = CreateInstance(typeStr);
                if (o == null)
                {
                    return null;
                }
                MemberInfo[] members = o.GetType().GetMembers();
                for (int j = 0; j < members.Length; j++)
                {
                    if (IsMINeedExport(members[j]))
                    {
                        BinaryNode node = currNode.SelectSingleNode(members[j].Name);
                        if (node != null)
                        {
                            try
                            {
                                object obj4 = GetObject(node);
                                if ((node != null) && (obj4 != null))
                                {
                                    SetMIValue(members[j], o, obj4);
                                }
                            }
                            catch (Exception exception)
                            {
                                Debug.LogError(string.Concat(new object[] { "Set field value failed! Field ", members[j].Name, " in ", o.GetType(), "e:", exception }));
                            }
                        }
                    }
                }
            }
        }
        return o;
    }

    [DebuggerHidden]
    public static IEnumerator GetObjectAsync(BinaryNode currNode, ObjectHolder holder)
    {
        <GetObjectAsync>c__IteratorB rb = new <GetObjectAsync>c__IteratorB();
        rb.currNode = currNode;
        rb.holder = holder;
        rb.<$>currNode = currNode;
        rb.<$>holder = holder;
        return rb;
    }

    public static string GetObjectHierachy(GameObject go)
    {
        string str = "/" + go.get_name();
        if (go.get_transform().get_parent() != null)
        {
            str = GetObjectHierachy(go.get_transform().get_parent().get_gameObject()) + str;
        }
        return str;
    }

    private static ICustomizedObjectSerializer GetObjectSerlizer(Type type)
    {
        ICustomizedObjectSerializer serializer = null;
        if (type.IsGenericType)
        {
            type = type.GetGenericTypeDefinition();
        }
        if (objectSerializerTypeCache.TryGetValue(type, out serializer))
        {
            return serializer;
        }
        return null;
    }

    private string GetPathFromGameObj(GameObject obj, string org_path)
    {
        string str = org_path;
        if (str.Contains(obj.get_transform().get_name()))
        {
            int startIndex = (str.IndexOf(obj.get_name()) + obj.get_name().Length) + 1;
            str = str.Substring(startIndex, (str.Length - startIndex) - 1);
        }
        return str;
    }

    public static string GetPureType(object o)
    {
        if (o is Component)
        {
            return GetPureType(o.GetType().ToString());
        }
        return o.GetType().ToString();
    }

    public static string GetPureType(string str)
    {
        string str2 = str;
        if (str2.Contains("."))
        {
            str2 = str2.Substring(str2.LastIndexOf(".") + 1, (str2.Length - str2.LastIndexOf(".")) - 1);
        }
        return str2;
    }

    public static object GetResource(string pathName, Type type)
    {
        object prefabObject = null;
        string fullPathInResources = pathName.Replace(@"\", "/");
        if (fullPathInResources.Contains("PrefabAssets/"))
        {
            PrefabRefAsset content = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(PrefabRefAsset), enResourceType.BattleScene, false, false).m_content;
            if (content != null)
            {
                prefabObject = content.m_prefabObject;
            }
            return prefabObject;
        }
        string str2 = pathName;
        string str3 = "Assets/Resources";
        int index = pathName.IndexOf(str3);
        if (index >= 0)
        {
            str2 = pathName.Substring((index + str3.Length) + 1);
        }
        return Singleton<CResourceManager>.GetInstance().GetResource(str2, type, enResourceType.BattleScene, false, false).m_content;
    }

    public static Type GetType(string typeStr)
    {
        Type type = null;
        if (s_typeCache.TryGetValue(typeStr, out type))
        {
            return type;
        }
        if ((typeStr.Contains("`") && typeStr.Contains("[")) && typeStr.Contains("]"))
        {
            string str = typeStr.Substring(0, typeStr.IndexOf("["));
            int index = typeStr.IndexOf("[");
            int num2 = typeStr.LastIndexOf("]");
            char[] separator = new char[] { ',' };
            string[] strArray = typeStr.Substring(index + 1, (num2 - index) - 1).Split(separator);
            Type type2 = GetType(str);
            Type[] typeArguments = new Type[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                typeArguments[i] = GetType(strArray[i]);
            }
            Type type3 = type2.MakeGenericType(typeArguments);
            s_typeCache.Add(typeStr, type3);
            return type3;
        }
        Type type4 = Utility.GetType(typeStr);
        if (type4 == null)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int j = 0; j < assemblies.Length; j++)
            {
                if (assemblies[j] != null)
                {
                    Type[] types = assemblies[j].GetTypes();
                    for (int k = 0; k < types.Length; k++)
                    {
                        if (typeStr == types[k].Name)
                        {
                            type4 = types[k];
                            s_typeCache.Add(typeStr, type4);
                            return type4;
                        }
                    }
                }
            }
        }
        s_typeCache.Add(typeStr, type4);
        return type4;
    }

    private static void InitComponets(BinaryNode domNode, GameObject go)
    {
        Component[] components = go.GetComponents(typeof(Component));
        for (int i = 0; i < domNode.GetChildNum(); i++)
        {
            BinaryNode child = domNode.GetChild(i);
            if (child.GetName() == "Cop")
            {
                string nodeAttr = GetNodeAttr(child, "Type");
                Component component = null;
                for (int j = 0; j < components.Length; j++)
                {
                    if (!isNull(components[j]) && GetPureType(components[j].GetType().ToString()).Equals(nodeAttr))
                    {
                        component = components[j];
                        break;
                    }
                }
                if (component == null)
                {
                    component = go.AddComponent(GetType(nodeAttr));
                }
            }
        }
    }

    private static bool IsComplexObject(object o)
    {
        if (o == null)
        {
            return false;
        }
        if ((o is GameObject) || (o is Component))
        {
            return false;
        }
        return ((!o.GetType().IsValueType && !(o is string)) && (o.GetType() != typeof(decimal)));
    }

    public static bool IsEqual(object o1, object o2)
    {
        if ((o1 != null) || (o2 != null))
        {
            if ((o1 == null) || (o2 == null))
            {
                return false;
            }
            if (o1.GetType() != o2.GetType())
            {
                return false;
            }
            if (IsComplexObject(o1) || IsComplexObject(o2))
            {
                return false;
            }
            if (IsGameObject(o1) && IsGameObject(o2))
            {
                GameObject go = (GameObject) o1;
                GameObject obj3 = (GameObject) o2;
                string gameObjectPathName = GetGameObjectPathName(go);
                string str2 = GetGameObjectPathName(obj3);
                return (((gameObjectPathName != null) && (str2 != null)) && gameObjectPathName.Equals(str2));
            }
            if (!object.ReferenceEquals(o1.GetType(), o2.GetType()))
            {
                return false;
            }
            if (!o1.GetType().ToString().EndsWith("[]"))
            {
                return o1.Equals(o2);
            }
            Array array = (Array) o1;
            Array array2 = (Array) o2;
            if (array.GetLength(0) != array2.GetLength(0))
            {
                return false;
            }
            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (array.GetValue(i) != array2.GetValue(i))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static bool IsGameObject(object obj)
    {
        return (obj is GameObject);
    }

    private static bool IsInherit(Type type, Type BaseType)
    {
        if (type == BaseType)
        {
            return true;
        }
        if (BaseType.IsInterface)
        {
            return BaseType.IsAssignableFrom(type);
        }
        if (BaseType.IsValueType)
        {
            return false;
        }
        if (BaseType == typeof(Enum))
        {
            return type.IsEnum;
        }
        return type.IsSubclassOf(BaseType);
    }

    private static bool IsMINeedExport(MemberInfo mi)
    {
        if (mi.MemberType == MemberTypes.Field)
        {
            FieldInfo info = (FieldInfo) mi;
            if (info.IsLiteral && !info.IsInitOnly)
            {
                return false;
            }
            if (info.IsStatic)
            {
                return false;
            }
            return true;
        }
        if (mi.MemberType != MemberTypes.Property)
        {
            return false;
        }
        PropertyInfo info2 = (PropertyInfo) mi;
        return ((info2.GetSetMethod() != null) && info2.GetSetMethod().IsPublic);
    }

    private static bool IsNeedNotSave(object o, bool storeAsObject)
    {
        return IsNeedNotSaveByType(o.GetType(), storeAsObject);
    }

    private static bool IsNeedNotSaveByType(Type tp, bool storeAsObject)
    {
        if (storeAsObject)
        {
            for (int j = 0; j < s_FieldsCannotSerialize.Length; j++)
            {
                if (IsInherit(tp, s_FieldsCannotSerialize[j]))
                {
                    return true;
                }
            }
            return false;
        }
        for (int i = 0; i < s_ComponentsCannotSerialize.Length; i++)
        {
            if (IsInherit(tp, s_ComponentsCannotSerialize[i]))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsNeedNotSaveMemberInfo(object o, MemberInfo mi, bool storeAsObject)
    {
        if (!IsNeedNotSaveByType(GetMIType(mi), storeAsObject))
        {
            if (mi is PropertyInfo)
            {
                PropertyInfo info = (PropertyInfo) mi;
                if (!info.CanWrite)
                {
                    return true;
                }
                if (info.GetIndexParameters().Length != 0)
                {
                    return true;
                }
            }
            if ((((o == null) || IsUnityReferenceType(o.GetType())) || (GetMITypeStr(mi).EndsWith("[]") || (GetMIValue(mi, o) != o))) && (((o == null) || !(o is Component)) || ((!mi.Name.Equals("tag") && !mi.Name.Equals("name")) && !mi.Name.Equals("active"))))
            {
                return false;
            }
        }
        return true;
    }

    private static bool isNull(object obj)
    {
        return (object.ReferenceEquals(obj, null) || obj.Equals(null));
    }

    private static bool IsPrimitive(Type type)
    {
        return ((type.IsPrimitive || (type == typeof(string))) || (type == typeof(decimal)));
    }

    public static bool isStringEqual(string s1, string s2)
    {
        return (((s1 != null) && (s2 != null)) && s1.Equals(s2));
    }

    private static bool IsUnityReferenceType(Type type)
    {
        return (((IsInherit(type, typeof(GameObject)) || IsInherit(type, typeof(Component))) || (IsInherit(type, typeof(ScriptableObject)) || IsInherit(type, typeof(Mesh)))) || IsInherit(type, typeof(Material)));
    }

    private static GameObject Load(BinaryDomDocument document)
    {
        GameObject go = null;
        s_gameObjectRoot4Read = null;
        BinaryNode root = document.Root;
        if (root != null)
        {
            go = LoadRecursionOnce(null, root);
            s_gameObjectRoot4Read = go;
            LoadRecursionTwice(null, root, go);
        }
        if (Camera.get_main() != null)
        {
            Camera.SetupCurrent(Camera.get_main());
        }
        s_gameObjectRoot4Read = null;
        return go;
    }

    public GameObject Load(LevelResAsset lightmapAsset)
    {
        GameObject obj2 = null;
        BinaryDomDocument document = null;
        if (lightmapAsset != null)
        {
            if (lightmapAsset.levelDom != null)
            {
                document = new BinaryDomDocument(lightmapAsset.levelDom.get_bytes());
                obj2 = Load(document);
            }
            else
            {
                return null;
            }
            int length = lightmapAsset.lightmapFar.Length;
            LightmapData[] dataArray = new LightmapData[length];
            for (int i = 0; i < length; i++)
            {
                LightmapData data = new LightmapData();
                data.set_lightmapFar(lightmapAsset.lightmapFar[i]);
                data.set_lightmapNear(lightmapAsset.lightmapNear[i]);
                dataArray[i] = data;
            }
            LightmapSettings.set_lightmaps(dataArray);
        }
        return obj2;
    }

    public GameObject Load(byte[] data)
    {
        BinaryDomDocument document = new BinaryDomDocument(data);
        return Load(document);
    }

    [DebuggerHidden]
    public IEnumerator LoadAsync(byte[] data, ObjectHolder holder)
    {
        <LoadAsync>c__Iterator7 iterator = new <LoadAsync>c__Iterator7();
        iterator.data = data;
        iterator.holder = holder;
        iterator.<$>data = data;
        iterator.<$>holder = holder;
        iterator.<>f__this = this;
        return iterator;
    }

    [DebuggerHidden]
    private static IEnumerator LoadAsync(BinaryDomDocument document, ObjectHolder holder)
    {
        <LoadAsync>c__Iterator5 iterator = new <LoadAsync>c__Iterator5();
        iterator.document = document;
        iterator.holder = holder;
        iterator.<$>document = document;
        iterator.<$>holder = holder;
        return iterator;
    }

    [DebuggerHidden]
    public IEnumerator LoadAsync(LevelResAsset lightmapAsset, ObjectHolder holder)
    {
        <LoadAsync>c__Iterator6 iterator = new <LoadAsync>c__Iterator6();
        iterator.lightmapAsset = lightmapAsset;
        iterator.holder = holder;
        iterator.<$>lightmapAsset = lightmapAsset;
        iterator.<$>holder = holder;
        iterator.<>f__this = this;
        return iterator;
    }

    private static void LoadComponets(BinaryNode domNode, GameObject go)
    {
        for (int i = 0; i < domNode.GetChildNum(); i++)
        {
            BinaryNode child = domNode.GetChild(i);
            if (child.GetName() == "Cop")
            {
                string nodeAttr = GetNodeAttr(child, "Type");
                Component cmp = go.GetComponent(nodeAttr);
                if (cmp != null)
                {
                    if ((GetNodeAttr(child, "DIS") != null) && (cmp is Behaviour))
                    {
                        cmp.set_enabled(false);
                    }
                    ICustomizedComponentSerializer componentSerlizer = GetComponentSerlizer(cmp.GetType());
                    if (componentSerlizer != null)
                    {
                        componentSerlizer.ComponentDeserialize(cmp, child);
                    }
                    else
                    {
                        MemberInfo[] members = cmp.GetType().GetMembers();
                        for (int j = 0; j < members.Length; j++)
                        {
                            if (IsMINeedExport(members[j]))
                            {
                                BinaryNode currNode = child.SelectSingleNode(members[j].Name);
                                if (currNode != null)
                                {
                                    object obj2 = GetObject(currNode);
                                    try
                                    {
                                        if (obj2 != null)
                                        {
                                            SetMIValue(members[j], cmp, obj2);
                                        }
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    [DebuggerHidden]
    private static IEnumerator LoadComponetsAsync(BinaryNode domNode, GameObject go)
    {
        <LoadComponetsAsync>c__IteratorA ra = new <LoadComponetsAsync>c__IteratorA();
        ra.domNode = domNode;
        ra.go = go;
        ra.<$>domNode = domNode;
        ra.<$>go = go;
        return ra;
    }

    private static GameObject LoadRecursionOnce(GameObject parentGo, BinaryNode domNode)
    {
        GameObject go = null;
        string nodeAttr = GetNodeAttr(domNode, "ON");
        if (parentGo != null)
        {
            for (int j = 0; j < parentGo.get_transform().get_childCount(); j++)
            {
                if (parentGo.get_transform().GetChild(j).get_name().Equals(nodeAttr))
                {
                    go = parentGo.get_transform().GetChild(j).get_gameObject();
                    break;
                }
            }
        }
        if (go == null)
        {
            string pathName = GetNodeAttr(domNode, "PFB");
            if ((pathName != null) && (pathName.Length != 0))
            {
                object resource = GetResource(pathName, typeof(GameObject));
                if ((resource == null) || !(resource is GameObject))
                {
                    Debug.LogError(pathName + " 不存在或者类型错误，请重新导出该场景");
                    go = new GameObject();
                }
                else
                {
                    GameObject obj4 = resource as GameObject;
                    bool flag = obj4.get_activeSelf();
                    obj4.SetActive(false);
                    go = Object.Instantiate(obj4);
                    obj4.SetActive(flag);
                }
            }
            else
            {
                go = new GameObject();
            }
        }
        Vector3 vector = go.get_transform().get_localScale();
        go.set_name(GetNodeAttr(domNode, "ON"));
        if (parentGo != null)
        {
            go.get_transform().set_parent(parentGo.get_transform());
        }
        go.get_transform().set_localScale(vector);
        DeserializeObject(domNode, go);
        go.SetActive(false);
        InitComponets(domNode, go);
        for (int i = 0; i < domNode.GetChildNum(); i++)
        {
            BinaryNode child = domNode.GetChild(i);
            if (child.GetName() == "CHD")
            {
                LoadRecursionOnce(go, child);
            }
        }
        return go;
    }

    [DebuggerHidden]
    private static IEnumerator LoadRecursionOnceAsync(GameObject parentGo, BinaryNode domNode, ObjectHolder result)
    {
        <LoadRecursionOnceAsync>c__Iterator8 iterator = new <LoadRecursionOnceAsync>c__Iterator8();
        iterator.domNode = domNode;
        iterator.parentGo = parentGo;
        iterator.result = result;
        iterator.<$>domNode = domNode;
        iterator.<$>parentGo = parentGo;
        iterator.<$>result = result;
        return iterator;
    }

    private static void LoadRecursionTwice(GameObject parentGo, BinaryNode domNode, GameObject go)
    {
        if (domNode == domNode.OwnerDocument.Root)
        {
            LoadComponets(domNode, go);
            int num = -1;
            for (int i = 0; i < domNode.GetChildNum(); i++)
            {
                BinaryNode child = domNode.GetChild(i);
                if (child.GetName() == "CHD")
                {
                    num++;
                    GameObject obj2 = go.get_transform().GetChild(num).get_gameObject();
                    if (obj2 != null)
                    {
                        LoadRecursionTwice(null, child, obj2);
                    }
                }
            }
        }
        else
        {
            BinaryNode parentNode = domNode.ParentNode;
            for (int j = 0; j < parentNode.GetChildNum(); j++)
            {
                BinaryNode node = parentNode.GetChild(j);
                if ((node.GetName() == "CHD") && (GetAttribute(node, "ON") == go.get_name()))
                {
                    LoadComponets(node, go);
                    if ((node.GetChildNum() > 0) && (go.get_transform().get_childCount() > 0))
                    {
                        BinaryNode node4 = node.GetChild(0);
                        for (int k = 0; k < go.get_transform().get_childCount(); k++)
                        {
                            GameObject obj3 = go.get_transform().GetChild(k).get_gameObject();
                            LoadRecursionTwice(null, node4, obj3);
                        }
                    }
                    domNode = node;
                }
            }
        }
        if (GetNodeAttr(domNode, "DIS") != null)
        {
            go.SetActive(false);
        }
        else
        {
            go.SetActive(true);
        }
    }

    [DebuggerHidden]
    private static IEnumerator LoadRecursionTwiceAsync(GameObject parentGo, BinaryNode domNode, GameObject go)
    {
        <LoadRecursionTwiceAsync>c__Iterator9 iterator = new <LoadRecursionTwiceAsync>c__Iterator9();
        iterator.domNode = domNode;
        iterator.go = go;
        iterator.<$>domNode = domNode;
        iterator.<$>go = go;
        return iterator;
    }

    private static void SetMIValue(MemberInfo mi, object owner, object value)
    {
        if (((owner != null) && (mi != null)) && !owner.ToString().Equals("null"))
        {
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo) mi).SetValue(owner, value);
                    break;

                case MemberTypes.Property:
                {
                    PropertyInfo info2 = (PropertyInfo) mi;
                    if (GetMIType(mi).ToString().EndsWith("[]"))
                    {
                        object[] objArray = (object[]) value;
                        IList list = (IList) info2.GetValue(owner, null);
                        for (int i = 0; i < objArray.Length; i++)
                        {
                            list[i] = objArray[i];
                        }
                    }
                    else
                    {
                        try
                        {
                            info2.SetValue(owner, value, null);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    return;
                }
            }
        }
    }

    private static DictionaryView<Type, ICustomizedComponentSerializer> componentSerializerTypeCache
    {
        get
        {
            if (s_componentSerializerTypeCache == null)
            {
                s_componentSerializerTypeCache = new DictionaryView<Type, ICustomizedComponentSerializer>();
                ClassEnumerator enumerator = new ClassEnumerator(typeof(ComponentTypeSerializerAttribute), typeof(ICustomizedComponentSerializer), typeof(ComponentTypeSerializerAttribute).Assembly, true, false, false);
                foreach (Type type in enumerator.results)
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(ComponentTypeSerializerAttribute), true);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        s_componentSerializerTypeCache.Add((customAttributes[0] as ComponentTypeSerializerAttribute).type, Activator.CreateInstance(type) as ICustomizedComponentSerializer);
                    }
                }
            }
            return s_componentSerializerTypeCache;
        }
    }

    private static DictionaryView<Type, ICustomizedObjectSerializer> objectSerializerTypeCache
    {
        get
        {
            if (s_objectSerializerTypeCache == null)
            {
                s_objectSerializerTypeCache = new DictionaryView<Type, ICustomizedObjectSerializer>();
                ClassEnumerator enumerator = new ClassEnumerator(typeof(ObjectTypeSerializerAttribute), typeof(ICustomizedObjectSerializer), typeof(ObjectTypeSerializerAttribute).Assembly, true, false, false);
                foreach (Type type in enumerator.results)
                {
                    object[] customAttributes = type.GetCustomAttributes(typeof(ObjectTypeSerializerAttribute), true);
                    for (int i = 0; i < customAttributes.Length; i++)
                    {
                        Type key = (customAttributes[0] as ObjectTypeSerializerAttribute).type;
                        if (key.IsGenericType)
                        {
                            key = key.GetGenericTypeDefinition();
                        }
                        s_objectSerializerTypeCache.Add(key, Activator.CreateInstance(type) as ICustomizedObjectSerializer);
                    }
                }
            }
            return s_objectSerializerTypeCache;
        }
    }

    [CompilerGenerated]
    private sealed class <GetObjectAsync>c__IteratorB : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BinaryNode <$>currNode;
        internal ObjectHolder <$>holder;
        internal Type <arrayType>__5;
        internal string <classJudgeTypeStr>__3;
        internal string <classTypeStr>__1;
        internal string <classValueStr>__2;
        internal BinaryNode <fieldNode>__16;
        internal int <i>__15;
        internal int <i>__7;
        internal string <isNull>__0;
        internal MemberInfo[] <mis>__14;
        internal Object <objMiddle>__12;
        internal string <pureType>__13;
        internal ICustomizedObjectSerializer <serializer>__10;
        internal string <singleTypeStr>__4;
        internal Array <tempArr>__6;
        internal ObjectHolder <tmpHolder>__17;
        internal ObjectHolder <tmpHolder>__8;
        internal Type <type>__11;
        internal Type <type>__9;
        internal BinaryNode currNode;
        internal ObjectHolder holder;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<isNull>__0 = GameSerializer.GetNodeAttr(this.currNode, "NULL");
                    if (this.<isNull>__0 == null)
                    {
                        this.<classTypeStr>__1 = GameSerializer.GetNodeAttr(this.currNode, "Type");
                        this.<classValueStr>__2 = GameSerializer.GetNodeAttr(this.currNode, "V");
                        this.<classJudgeTypeStr>__3 = GameSerializer.GetNodeAttr(this.currNode, "JT");
                        if ("Arr".Equals(this.<classJudgeTypeStr>__3))
                        {
                            if (this.<classTypeStr>__1 != null)
                            {
                                this.<singleTypeStr>__4 = this.<classTypeStr>__1.Replace("[]", string.Empty);
                                this.<arrayType>__5 = GameSerializer.GetType(this.<singleTypeStr>__4);
                                if (this.<arrayType>__5 == null)
                                {
                                    Debug.LogError("Array type " + this.<classTypeStr>__1 + " create failed!");
                                    this.holder.obj = null;
                                    break;
                                }
                                this.<tempArr>__6 = Array.CreateInstance(this.<arrayType>__5, this.currNode.GetChildNum());
                                this.<i>__7 = 0;
                                while (this.<i>__7 < this.<tempArr>__6.Length)
                                {
                                    this.<tmpHolder>__8 = new ObjectHolder();
                                    this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.GetObjectAsync(this.currNode.GetChild(this.<i>__7), this.<tmpHolder>__8));
                                    this.$PC = 1;
                                    goto Label_05D4;
                                Label_0186:
                                    this.<tempArr>__6.SetValue(this.<tmpHolder>__8.obj, this.<i>__7);
                                    this.<i>__7++;
                                }
                                this.holder.obj = this.<tempArr>__6;
                            }
                        }
                        else if ("Cus".Equals(this.<classJudgeTypeStr>__3))
                        {
                            if (this.<classTypeStr>__1 != null)
                            {
                                this.<type>__9 = GameSerializer.GetType(this.<classTypeStr>__1);
                                this.<serializer>__10 = GameSerializer.GetObjectSerlizer(this.<type>__9);
                                if ((this.<serializer>__10 != null) && (this.<serializer>__10 is ICustomInstantiate))
                                {
                                    this.holder.obj = ((ICustomInstantiate) this.<serializer>__10).Instantiate(this.currNode);
                                }
                                else
                                {
                                    this.holder.obj = GameSerializer.CreateInstance(this.<type>__9);
                                }
                                if (this.holder.obj == null)
                                {
                                    break;
                                }
                                if (this.<serializer>__10 != null)
                                {
                                    this.<serializer>__10.ObjectDeserialize(ref this.holder.obj, this.currNode);
                                }
                            }
                        }
                        else if ("Enum".Equals(this.<classJudgeTypeStr>__3))
                        {
                            if (this.<classTypeStr>__1 != null)
                            {
                                this.<type>__11 = GameSerializer.GetType(this.<classTypeStr>__1);
                                this.holder.obj = Enum.ToObject(this.<type>__11, int.Parse(this.<classValueStr>__2));
                            }
                        }
                        else if ("Pri".Equals(this.<classJudgeTypeStr>__3))
                        {
                            if (this.<classTypeStr>__1 != null)
                            {
                                this.holder.obj = Convert.ChangeType(this.<classValueStr>__2, GameSerializer.GetType(this.<classTypeStr>__1));
                            }
                        }
                        else if ("Ref".Equals(this.<classJudgeTypeStr>__3))
                        {
                            this.<objMiddle>__12 = GameSerializer.GetGameObjectFromPath(this.<classValueStr>__2, this.<classTypeStr>__1);
                            if (this.<objMiddle>__12 != null)
                            {
                                if (this.<objMiddle>__12 is GameObject)
                                {
                                    if (this.<classTypeStr>__1 != null)
                                    {
                                        this.<pureType>__13 = GameSerializer.GetPureType(this.<classTypeStr>__1);
                                        if (!"GameObject".Equals(this.<pureType>__13))
                                        {
                                            this.holder.obj = (this.<objMiddle>__12 as GameObject).GetComponent(this.<pureType>__13);
                                            if (this.holder.obj == null)
                                            {
                                                Debug.LogError("No " + this.<pureType>__13 + " component found in " + this.<classValueStr>__2);
                                            }
                                        }
                                        else
                                        {
                                            this.holder.obj = this.<objMiddle>__12;
                                        }
                                    }
                                }
                                else
                                {
                                    this.holder.obj = this.<objMiddle>__12;
                                }
                            }
                            else
                            {
                                this.holder.obj = null;
                                Debug.LogError("Load gameobject " + this.<classValueStr>__2 + " failed!");
                            }
                        }
                        else if ("Com".Equals(this.<classJudgeTypeStr>__3))
                        {
                            this.holder.obj = GameSerializer.CreateInstance(this.<classTypeStr>__1);
                            if (this.holder.obj == null)
                            {
                                break;
                            }
                            this.<mis>__14 = this.holder.obj.GetType().GetMembers();
                            this.<i>__15 = 0;
                            while (this.<i>__15 < this.<mis>__14.Length)
                            {
                                if (!GameSerializer.IsMINeedExport(this.<mis>__14[this.<i>__15]))
                                {
                                    goto Label_05AA;
                                }
                                this.<fieldNode>__16 = this.currNode.SelectSingleNode(this.<mis>__14[this.<i>__15].Name);
                                if (this.<fieldNode>__16 == null)
                                {
                                    goto Label_05AA;
                                }
                                this.<tmpHolder>__17 = new ObjectHolder();
                                this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.GetObjectAsync(this.<fieldNode>__16, this.<tmpHolder>__17));
                                this.$PC = 2;
                                goto Label_05D4;
                            Label_0567:
                                if ((this.<fieldNode>__16 != null) && (this.<tmpHolder>__17.obj != null))
                                {
                                    GameSerializer.SetMIValue(this.<mis>__14[this.<i>__15], this.holder.obj, this.<tmpHolder>__17.obj);
                                }
                            Label_05AA:
                                this.<i>__15++;
                            }
                        }
                        this.$PC = -1;
                        break;
                    }
                    this.holder.obj = null;
                    break;

                case 1:
                    goto Label_0186;

                case 2:
                    goto Label_0567;
            }
            return false;
        Label_05D4:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <LoadAsync>c__Iterator5 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BinaryDomDocument <$>document;
        internal ObjectHolder <$>holder;
        internal BinaryNode <rootNode>__0;
        internal BinaryDomDocument document;
        internal ObjectHolder holder;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    GameSerializer.s_gameObjectRoot4Read = null;
                    this.<rootNode>__0 = this.document.Root;
                    if (this.<rootNode>__0 == null)
                    {
                        break;
                    }
                    this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadRecursionOnceAsync(null, this.<rootNode>__0, this.holder));
                    this.$PC = 1;
                    goto Label_00E0;

                case 1:
                    GameSerializer.s_gameObjectRoot4Read = this.holder.obj as GameObject;
                    this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadRecursionTwiceAsync(null, this.<rootNode>__0, GameSerializer.s_gameObjectRoot4Read));
                    this.$PC = 2;
                    goto Label_00E0;

                case 2:
                    break;

                default:
                    goto Label_00DE;
            }
            if (Camera.get_main() != null)
            {
                Camera.SetupCurrent(Camera.get_main());
            }
            GameSerializer.s_gameObjectRoot4Read = null;
            this.$PC = -1;
        Label_00DE:
            return false;
        Label_00E0:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <LoadAsync>c__Iterator6 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal ObjectHolder <$>holder;
        internal LevelResAsset <$>lightmapAsset;
        internal GameSerializer <>f__this;
        internal int <Count>__2;
        internal BinaryDomDocument <document>__0;
        internal int <i>__4;
        internal LightmapData <Lightmap>__5;
        internal LightmapData[] <lightmapDatas>__3;
        internal TextAsset <ts>__1;
        internal ObjectHolder holder;
        internal LevelResAsset lightmapAsset;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<document>__0 = null;
                    if (this.lightmapAsset == null)
                    {
                        break;
                    }
                    if (this.lightmapAsset.levelDom == null)
                    {
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 2;
                    }
                    else
                    {
                        this.<ts>__1 = this.lightmapAsset.levelDom;
                        this.<document>__0 = new BinaryDomDocument(this.<ts>__1.get_bytes());
                        this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadAsync(this.<document>__0, this.holder));
                        this.$PC = 1;
                    }
                    goto Label_01A8;

                case 1:
                case 2:
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 3;
                    goto Label_01A8;

                case 3:
                    this.<Count>__2 = this.lightmapAsset.lightmapFar.Length;
                    this.<lightmapDatas>__3 = new LightmapData[this.<Count>__2];
                    this.<i>__4 = 0;
                    while (this.<i>__4 < this.<Count>__2)
                    {
                        this.<Lightmap>__5 = new LightmapData();
                        this.<Lightmap>__5.set_lightmapFar(this.lightmapAsset.lightmapFar[this.<i>__4]);
                        this.<Lightmap>__5.set_lightmapNear(this.lightmapAsset.lightmapNear[this.<i>__4]);
                        this.<lightmapDatas>__3[this.<i>__4] = this.<Lightmap>__5;
                        this.<i>__4++;
                    }
                    LightmapSettings.set_lightmaps(this.<lightmapDatas>__3);
                    break;

                default:
                    goto Label_01A6;
            }
            this.$PC = -1;
        Label_01A6:
            return false;
        Label_01A8:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <LoadAsync>c__Iterator7 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal byte[] <$>data;
        internal ObjectHolder <$>holder;
        internal GameSerializer <>f__this;
        internal BinaryDomDocument <document>__1;
        internal GameObject <go>__0;
        internal byte[] data;
        internal ObjectHolder holder;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<go>__0 = null;
                    this.<document>__1 = new BinaryDomDocument(this.data);
                    this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadAsync(this.<document>__1, this.holder));
                    this.$PC = 1;
                    return true;

                case 1:
                    this.$PC = -1;
                    break;
            }
            return false;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <LoadComponetsAsync>c__IteratorA : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BinaryNode <$>domNode;
        internal GameObject <$>go;
        internal Behaviour <behaviour>__5;
        internal BinaryNode <childNode>__1;
        internal string <classType>__2;
        internal Component <cpnt>__3;
        internal string <disableMark>__4;
        internal Exception <e>__11;
        internal BinaryNode <fieldNode>__9;
        internal ObjectHolder <fieldObject>__10;
        internal int <i>__0;
        internal int <j>__8;
        internal MemberInfo[] <mis>__7;
        internal ICustomizedComponentSerializer <serializer>__6;
        internal BinaryNode domNode;
        internal GameObject go;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<i>__0 = 0;
                    goto Label_0289;

                case 1:
                    goto Label_027B;

                case 2:
                    goto Label_01F9;

                case 3:
                    goto Label_025A;

                default:
                    goto Label_02A6;
            }
        Label_027B:
            this.<i>__0++;
        Label_0289:
            if (this.<i>__0 < this.domNode.GetChildNum())
            {
                this.<childNode>__1 = this.domNode.GetChild(this.<i>__0);
                if (this.<childNode>__1.GetName() == "Cop")
                {
                    this.<classType>__2 = GameSerializer.GetNodeAttr(this.<childNode>__1, "Type");
                    this.<cpnt>__3 = this.go.GetComponent(this.<classType>__2);
                    if (this.<cpnt>__3 != null)
                    {
                        this.<disableMark>__4 = GameSerializer.GetNodeAttr(this.<childNode>__1, "DIS");
                        if ((this.<disableMark>__4 != null) && (this.<cpnt>__3 is Behaviour))
                        {
                            this.<behaviour>__5 = this.<cpnt>__3;
                            this.<behaviour>__5.set_enabled(false);
                        }
                        this.<serializer>__6 = GameSerializer.GetComponentSerlizer(this.<cpnt>__3.GetType());
                        if (this.<serializer>__6 != null)
                        {
                            this.<serializer>__6.ComponentDeserialize(this.<cpnt>__3, this.<childNode>__1);
                            this.$current = new CHoldForSecond(0f);
                            this.$PC = 1;
                            goto Label_02A8;
                        }
                        this.<mis>__7 = this.<cpnt>__3.GetType().GetMembers();
                        this.<j>__8 = 0;
                        while (this.<j>__8 < this.<mis>__7.Length)
                        {
                            if (!GameSerializer.IsMINeedExport(this.<mis>__7[this.<j>__8]))
                            {
                                goto Label_025A;
                            }
                            this.<fieldNode>__9 = this.<childNode>__1.SelectSingleNode(this.<mis>__7[this.<j>__8].Name);
                            if (this.<fieldNode>__9 == null)
                            {
                                goto Label_023E;
                            }
                            this.<fieldObject>__10 = new ObjectHolder();
                            this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.GetObjectAsync(this.<fieldNode>__9, this.<fieldObject>__10));
                            this.$PC = 2;
                            goto Label_02A8;
                        Label_01F9:
                            try
                            {
                                if (this.<fieldObject>__10.obj != null)
                                {
                                    GameSerializer.SetMIValue(this.<mis>__7[this.<j>__8], this.<cpnt>__3, this.<fieldObject>__10.obj);
                                }
                            }
                            catch (Exception exception)
                            {
                                this.<e>__11 = exception;
                            }
                        Label_023E:
                            this.$current = new CHoldForSecond(0f);
                            this.$PC = 3;
                            goto Label_02A8;
                        Label_025A:
                            this.<j>__8++;
                        }
                    }
                }
                goto Label_027B;
            }
            this.$PC = -1;
        Label_02A6:
            return false;
        Label_02A8:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <LoadRecursionOnceAsync>c__Iterator8 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BinaryNode <$>domNode;
        internal GameObject <$>parentGo;
        internal ObjectHolder <$>result;
        internal BinaryNode <child>__9;
        internal GameObject <go>__0;
        internal string <goName>__1;
        internal GameObject <goRef>__5;
        internal ObjectHolder <holder>__10;
        internal int <i>__2;
        internal int <i>__8;
        internal bool <isPrefabActive>__6;
        internal object <oRef>__4;
        internal string <prefabRef>__3;
        internal Vector3 <v3Scale>__7;
        internal BinaryNode domNode;
        internal GameObject parentGo;
        internal ObjectHolder result;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    this.<go>__0 = null;
                    this.<goName>__1 = GameSerializer.GetNodeAttr(this.domNode, "ON");
                    if (this.parentGo != null)
                    {
                        this.<i>__2 = 0;
                        while (this.<i>__2 < this.parentGo.get_transform().get_childCount())
                        {
                            if (this.parentGo.get_transform().GetChild(this.<i>__2).get_name().Equals(this.<goName>__1))
                            {
                                this.<go>__0 = this.parentGo.get_transform().GetChild(this.<i>__2).get_gameObject();
                                break;
                            }
                            this.<i>__2++;
                        }
                    }
                    break;

                case 1:
                    if (this.<go>__0 != null)
                    {
                        goto Label_0243;
                    }
                    this.<prefabRef>__3 = GameSerializer.GetNodeAttr(this.domNode, "PFB");
                    if ((this.<prefabRef>__3 == null) || (this.<prefabRef>__3.Length == 0))
                    {
                        this.<go>__0 = new GameObject();
                        goto Label_0243;
                    }
                    this.<oRef>__4 = GameSerializer.GetResource(this.<prefabRef>__3, typeof(GameObject));
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 2;
                    goto Label_03F1;

                case 2:
                    if ((this.<oRef>__4 != null) && (this.<oRef>__4 is GameObject))
                    {
                        this.<goRef>__5 = this.<oRef>__4 as GameObject;
                        this.<isPrefabActive>__6 = this.<goRef>__5.get_activeSelf();
                        this.<goRef>__5.SetActive(false);
                        this.<go>__0 = Object.Instantiate(this.<goRef>__5);
                        this.<goRef>__5.SetActive(this.<isPrefabActive>__6);
                        this.$current = new CHoldForSecond(0f);
                        this.$PC = 3;
                        goto Label_03F1;
                    }
                    Debug.LogError(this.<prefabRef>__3 + " 不存在或者类型错误，请重新导出该场景");
                    this.<go>__0 = new GameObject();
                    goto Label_0243;

                case 3:
                    goto Label_0243;

                case 4:
                    this.<go>__0.SetActive(false);
                    GameSerializer.InitComponets(this.domNode, this.<go>__0);
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 5;
                    goto Label_03F1;

                case 5:
                    this.<i>__8 = 0;
                    goto Label_03A5;

                case 6:
                    goto Label_0397;

                case 7:
                    this.$PC = -1;
                    goto Label_03EF;

                default:
                    goto Label_03EF;
            }
            this.$current = new CHoldForSecond(0f);
            this.$PC = 1;
            goto Label_03F1;
        Label_0243:
            this.<v3Scale>__7 = this.<go>__0.get_transform().get_localScale();
            this.<go>__0.set_name(GameSerializer.GetNodeAttr(this.domNode, "ON"));
            if (this.parentGo != null)
            {
                this.<go>__0.get_transform().set_parent(this.parentGo.get_transform());
            }
            this.<go>__0.get_transform().set_localScale(this.<v3Scale>__7);
            GameSerializer.DeserializeObject(this.domNode, this.<go>__0);
            this.$current = new CHoldForSecond(0f);
            this.$PC = 4;
            goto Label_03F1;
        Label_0397:
            this.<i>__8++;
        Label_03A5:
            if (this.<i>__8 < this.domNode.GetChildNum())
            {
                this.<child>__9 = this.domNode.GetChild(this.<i>__8);
                if (this.<child>__9.GetName() == "CHD")
                {
                    this.<holder>__10 = new ObjectHolder();
                    this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadRecursionOnceAsync(this.<go>__0, this.<child>__9, this.<holder>__10));
                    this.$PC = 6;
                    goto Label_03F1;
                }
                goto Label_0397;
            }
            this.result.obj = this.<go>__0;
            this.$current = new CHoldForSecond(0f);
            this.$PC = 7;
            goto Label_03F1;
        Label_03EF:
            return false;
        Label_03F1:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <LoadRecursionTwiceAsync>c__Iterator9 : IDisposable, IEnumerator, IEnumerator<object>
    {
        internal object $current;
        internal int $PC;
        internal BinaryNode <$>domNode;
        internal GameObject <$>go;
        internal BinaryNode <child>__2;
        internal string <disableMark>__11;
        internal BinaryNode <dummyNode>__8;
        internal int <i>__1;
        internal int <i>__5;
        internal int <j>__9;
        internal GameObject <newGo>__10;
        internal GameObject <newGo>__3;
        internal int <num>__0;
        internal BinaryNode <parentNode>__4;
        internal BinaryNode <sibling>__6;
        internal string <siblingObjName>__7;
        internal BinaryNode domNode;
        internal GameObject go;

        [DebuggerHidden]
        public void Dispose()
        {
            this.$PC = -1;
        }

        public bool MoveNext()
        {
            uint num = (uint) this.$PC;
            this.$PC = -1;
            switch (num)
            {
                case 0:
                    if (this.domNode != this.domNode.OwnerDocument.Root)
                    {
                        this.<parentNode>__4 = this.domNode.ParentNode;
                        this.<i>__5 = 0;
                        while (this.<i>__5 < this.<parentNode>__4.GetChildNum())
                        {
                            this.<sibling>__6 = this.<parentNode>__4.GetChild(this.<i>__5);
                            if (this.<sibling>__6.GetName() != "CHD")
                            {
                                goto Label_02FE;
                            }
                            this.<siblingObjName>__7 = GameSerializer.GetAttribute(this.<sibling>__6, "ON");
                            if (this.<siblingObjName>__7 != this.go.get_name())
                            {
                                goto Label_02FE;
                            }
                            this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadComponetsAsync(this.<sibling>__6, this.go));
                            this.$PC = 3;
                            goto Label_0369;
                        Label_027A:
                            this.<newGo>__10 = this.go.get_transform().GetChild(this.<j>__9).get_gameObject();
                            this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadRecursionTwiceAsync(null, this.<dummyNode>__8, this.<newGo>__10));
                            this.$PC = 5;
                            goto Label_0369;
                        Label_02D7:
                            if (this.<j>__9 < this.go.get_transform().get_childCount())
                            {
                                goto Label_027A;
                            }
                        Label_02F2:
                            this.domNode = this.<sibling>__6;
                        Label_02FE:
                            this.<i>__5++;
                        }
                        goto Label_0322;
                    }
                    this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadComponetsAsync(this.domNode, this.go));
                    this.$PC = 1;
                    goto Label_0369;

                case 1:
                    this.<num>__0 = -1;
                    this.<i>__1 = 0;
                    goto Label_0148;

                case 2:
                    goto Label_013A;

                case 3:
                    this.$current = new CHoldForSecond(0f);
                    this.$PC = 4;
                    goto Label_0369;

                case 4:
                    if ((this.<sibling>__6.GetChildNum() <= 0) || (this.go.get_transform().get_childCount() <= 0))
                    {
                        goto Label_02F2;
                    }
                    this.<dummyNode>__8 = this.<sibling>__6.GetChild(0);
                    this.<j>__9 = 0;
                    goto Label_02D7;

                case 5:
                    this.<j>__9++;
                    goto Label_02D7;

                default:
                    goto Label_0367;
            }
        Label_013A:
            this.<i>__1++;
        Label_0148:
            if (this.<i>__1 < this.domNode.GetChildNum())
            {
                this.<child>__2 = this.domNode.GetChild(this.<i>__1);
                if (this.<child>__2.GetName() == "CHD")
                {
                    this.<num>__0++;
                    this.<newGo>__3 = this.go.get_transform().GetChild(this.<num>__0).get_gameObject();
                    if (this.<newGo>__3 != null)
                    {
                        this.$current = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameSerializer.LoadRecursionTwiceAsync(null, this.<child>__2, this.<newGo>__3));
                        this.$PC = 2;
                        goto Label_0369;
                    }
                }
                goto Label_013A;
            }
        Label_0322:
            this.<disableMark>__11 = GameSerializer.GetNodeAttr(this.domNode, "DIS");
            if (this.<disableMark>__11 != null)
            {
                this.go.SetActive(false);
            }
            else
            {
                this.go.SetActive(true);
            }
            this.$PC = -1;
        Label_0367:
            return false;
        Label_0369:
            return true;
        }

        [DebuggerHidden]
        public void Reset()
        {
            throw new NotSupportedException();
        }

        object IEnumerator<object>.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return this.$current;
            }
        }
    }
}

