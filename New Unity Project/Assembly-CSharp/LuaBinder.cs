using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static class LuaBinder
{
    [CompilerGenerated]
    private static Dictionary<string, int> <>f__switch$map1;
    public static List<string> wrapList = new List<string>();

    public static void Bind(IntPtr L, string type = new string())
    {
        if ((type != null) && !wrapList.Contains(type))
        {
            wrapList.Add(type);
            type = type + "Wrap";
            string key = type;
            if (key != null)
            {
                int num;
                if (<>f__switch$map1 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(0x43);
                    dictionary.Add("AnimationBlendModeWrap", 0);
                    dictionary.Add("AnimationClipWrap", 1);
                    dictionary.Add("AnimationStateWrap", 2);
                    dictionary.Add("AnimationWrap", 3);
                    dictionary.Add("ApplicationWrap", 4);
                    dictionary.Add("AssetBundleWrap", 5);
                    dictionary.Add("AsyncOperationWrap", 6);
                    dictionary.Add("BehaviourWrap", 7);
                    dictionary.Add("BlendWeightsWrap", 8);
                    dictionary.Add("ComponentWrap", 9);
                    dictionary.Add("com_tencent_pandora_AppConstWrap", 10);
                    dictionary.Add("com_tencent_pandora_CUserDataWrap", 11);
                    dictionary.Add("com_tencent_pandora_DelegateFactoryWrap", 12);
                    dictionary.Add("com_tencent_pandora_GetNewsImageWrap", 13);
                    dictionary.Add("com_tencent_pandora_LoggerWrap", 14);
                    dictionary.Add("com_tencent_pandora_LuaBehaviourWrap", 15);
                    dictionary.Add("com_tencent_pandora_LuaHelperWrap", 0x10);
                    dictionary.Add("com_tencent_pandora_NetProxcyWrap", 0x11);
                    dictionary.Add("com_tencent_pandora_NotificationCenterWrap", 0x12);
                    dictionary.Add("com_tencent_pandora_PanelManagerWrap", 0x13);
                    dictionary.Add("com_tencent_pandora_PdrWrap", 20);
                    dictionary.Add("com_tencent_pandora_ResourceManagerWrap", 0x15);
                    dictionary.Add("com_tencent_pandora_ThirdSDKWrap", 0x16);
                    dictionary.Add("com_tencent_pandora_TimerManagerWrap", 0x17);
                    dictionary.Add("com_tencent_pandora_UtilWrap", 0x18);
                    dictionary.Add("DebuggerWrap", 0x19);
                    dictionary.Add("EnumWrap", 0x1a);
                    dictionary.Add("GameObjectWrap", 0x1b);
                    dictionary.Add("IEnumeratorWrap", 0x1c);
                    dictionary.Add("InputWrap", 0x1d);
                    dictionary.Add("KeyCodeWrap", 30);
                    dictionary.Add("MaterialWrap", 0x1f);
                    dictionary.Add("MeshRendererWrap", 0x20);
                    dictionary.Add("MonoBehaviourWrap", 0x21);
                    dictionary.Add("ObjectWrap", 0x22);
                    dictionary.Add("ParticleSystemWrap", 0x23);
                    dictionary.Add("QualitySettingsWrap", 0x24);
                    dictionary.Add("RendererWrap", 0x25);
                    dictionary.Add("RenderSettingsWrap", 0x26);
                    dictionary.Add("RenderTextureWrap", 0x27);
                    dictionary.Add("ScreenWrap", 40);
                    dictionary.Add("SkinnedMeshRendererWrap", 0x29);
                    dictionary.Add("SpaceWrap", 0x2a);
                    dictionary.Add("stringWrap", 0x2b);
                    dictionary.Add("System_ObjectWrap", 0x2c);
                    dictionary.Add("TextureWrap", 0x2d);
                    dictionary.Add("TimeWrap", 0x2e);
                    dictionary.Add("TouchPhaseWrap", 0x2f);
                    dictionary.Add("TrackedReferenceWrap", 0x30);
                    dictionary.Add("TransformWrap", 0x31);
                    dictionary.Add("TypeWrap", 50);
                    dictionary.Add("UnityEngine_EventSystems_UIBehaviourWrap", 0x33);
                    dictionary.Add("UnityEngine_Events_UnityEventBaseWrap", 0x34);
                    dictionary.Add("UnityEngine_Events_UnityEventWrap", 0x35);
                    dictionary.Add("UnityEngine_UI_ButtonWrap", 0x36);
                    dictionary.Add("UnityEngine_UI_Button_ButtonClickedEventWrap", 0x37);
                    dictionary.Add("UnityEngine_UI_GraphicWrap", 0x38);
                    dictionary.Add("UnityEngine_UI_ImageWrap", 0x39);
                    dictionary.Add("UnityEngine_UI_MaskableGraphicWrap", 0x3a);
                    dictionary.Add("UnityEngine_UI_RawImageWrap", 0x3b);
                    dictionary.Add("UnityEngine_UI_ScrollbarWrap", 60);
                    dictionary.Add("UnityEngine_UI_ScrollRectWrap", 0x3d);
                    dictionary.Add("UnityEngine_UI_SelectableWrap", 0x3e);
                    dictionary.Add("UnityEngine_UI_TextWrap", 0x3f);
                    dictionary.Add("UnityEngine_UI_ToggleWrap", 0x40);
                    dictionary.Add("UnityEngine_UI_VerticalLayoutGroupWrap", 0x41);
                    dictionary.Add("WWWWrap", 0x42);
                    <>f__switch$map1 = dictionary;
                }
                if (<>f__switch$map1.TryGetValue(key, out num))
                {
                    switch (num)
                    {
                        case 0:
                            AnimationBlendModeWrap.Register(L);
                            break;

                        case 1:
                            AnimationClipWrap.Register(L);
                            break;

                        case 2:
                            AnimationStateWrap.Register(L);
                            break;

                        case 3:
                            AnimationWrap.Register(L);
                            break;

                        case 4:
                            ApplicationWrap.Register(L);
                            break;

                        case 5:
                            AssetBundleWrap.Register(L);
                            break;

                        case 6:
                            AsyncOperationWrap.Register(L);
                            break;

                        case 7:
                            BehaviourWrap.Register(L);
                            break;

                        case 8:
                            BlendWeightsWrap.Register(L);
                            break;

                        case 9:
                            ComponentWrap.Register(L);
                            break;

                        case 10:
                            com_tencent_pandora_AppConstWrap.Register(L);
                            break;

                        case 11:
                            com_tencent_pandora_CUserDataWrap.Register(L);
                            break;

                        case 12:
                            com_tencent_pandora_DelegateFactoryWrap.Register(L);
                            break;

                        case 13:
                            com_tencent_pandora_GetNewsImageWrap.Register(L);
                            break;

                        case 14:
                            com_tencent_pandora_LoggerWrap.Register(L);
                            break;

                        case 15:
                            com_tencent_pandora_LuaBehaviourWrap.Register(L);
                            break;

                        case 0x10:
                            com_tencent_pandora_LuaHelperWrap.Register(L);
                            break;

                        case 0x11:
                            com_tencent_pandora_NetProxcyWrap.Register(L);
                            break;

                        case 0x12:
                            com_tencent_pandora_NotificationCenterWrap.Register(L);
                            break;

                        case 0x13:
                            com_tencent_pandora_PanelManagerWrap.Register(L);
                            break;

                        case 20:
                            com_tencent_pandora_PdrWrap.Register(L);
                            break;

                        case 0x15:
                            com_tencent_pandora_ResourceManagerWrap.Register(L);
                            break;

                        case 0x16:
                            com_tencent_pandora_ThirdSDKWrap.Register(L);
                            break;

                        case 0x17:
                            com_tencent_pandora_TimerManagerWrap.Register(L);
                            break;

                        case 0x18:
                            com_tencent_pandora_UtilWrap.Register(L);
                            break;

                        case 0x19:
                            DebuggerWrap.Register(L);
                            break;

                        case 0x1a:
                            EnumWrap.Register(L);
                            break;

                        case 0x1b:
                            GameObjectWrap.Register(L);
                            break;

                        case 0x1c:
                            IEnumeratorWrap.Register(L);
                            break;

                        case 0x1d:
                            InputWrap.Register(L);
                            break;

                        case 30:
                            KeyCodeWrap.Register(L);
                            break;

                        case 0x1f:
                            MaterialWrap.Register(L);
                            break;

                        case 0x20:
                            MeshRendererWrap.Register(L);
                            break;

                        case 0x21:
                            MonoBehaviourWrap.Register(L);
                            break;

                        case 0x22:
                            ObjectWrap.Register(L);
                            break;

                        case 0x23:
                            ParticleSystemWrap.Register(L);
                            break;

                        case 0x24:
                            QualitySettingsWrap.Register(L);
                            break;

                        case 0x25:
                            RendererWrap.Register(L);
                            break;

                        case 0x26:
                            RenderSettingsWrap.Register(L);
                            break;

                        case 0x27:
                            RenderTextureWrap.Register(L);
                            break;

                        case 40:
                            ScreenWrap.Register(L);
                            break;

                        case 0x29:
                            SkinnedMeshRendererWrap.Register(L);
                            break;

                        case 0x2a:
                            SpaceWrap.Register(L);
                            break;

                        case 0x2b:
                            stringWrap.Register(L);
                            break;

                        case 0x2c:
                            System_ObjectWrap.Register(L);
                            break;

                        case 0x2d:
                            TextureWrap.Register(L);
                            break;

                        case 0x2e:
                            TimeWrap.Register(L);
                            break;

                        case 0x2f:
                            TouchPhaseWrap.Register(L);
                            break;

                        case 0x30:
                            TrackedReferenceWrap.Register(L);
                            break;

                        case 0x31:
                            TransformWrap.Register(L);
                            break;

                        case 50:
                            TypeWrap.Register(L);
                            break;

                        case 0x33:
                            UnityEngine_EventSystems_UIBehaviourWrap.Register(L);
                            break;

                        case 0x34:
                            UnityEngine_Events_UnityEventBaseWrap.Register(L);
                            break;

                        case 0x35:
                            UnityEngine_Events_UnityEventWrap.Register(L);
                            break;

                        case 0x36:
                            UnityEngine_UI_ButtonWrap.Register(L);
                            break;

                        case 0x37:
                            UnityEngine_UI_Button_ButtonClickedEventWrap.Register(L);
                            break;

                        case 0x38:
                            UnityEngine_UI_GraphicWrap.Register(L);
                            break;

                        case 0x39:
                            UnityEngine_UI_ImageWrap.Register(L);
                            break;

                        case 0x3a:
                            UnityEngine_UI_MaskableGraphicWrap.Register(L);
                            break;

                        case 0x3b:
                            UnityEngine_UI_RawImageWrap.Register(L);
                            break;

                        case 60:
                            UnityEngine_UI_ScrollbarWrap.Register(L);
                            break;

                        case 0x3d:
                            UnityEngine_UI_ScrollRectWrap.Register(L);
                            break;

                        case 0x3e:
                            UnityEngine_UI_SelectableWrap.Register(L);
                            break;

                        case 0x3f:
                            UnityEngine_UI_TextWrap.Register(L);
                            break;

                        case 0x40:
                            UnityEngine_UI_ToggleWrap.Register(L);
                            break;

                        case 0x41:
                            UnityEngine_UI_VerticalLayoutGroupWrap.Register(L);
                            break;

                        case 0x42:
                            WWWWrap.Register(L);
                            break;
                    }
                }
            }
        }
    }
}

