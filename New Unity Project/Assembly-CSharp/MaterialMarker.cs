using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MaterialMarker : TemplateMarkerBase
{
    [Tooltip("材质的命名规则，不填表示不检测")]
    public TemplateMarkerBase.NamePattern m_materialNamePattern;
    [Tooltip("shader参数设置，填入想要检测的参数的个数")]
    public ShaderCheckParam[] m_shaderCheckParams;
    [Tooltip("Shader的命名规则，不填表示不检测")]
    public TemplateMarkerBase.NamePattern[] m_shaderNamePatterns;
    [Tooltip("贴图参数设置，填入该材质需要检测的贴图的张数")]
    public TextureCheckParam[] m_texCheckParams;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        Renderer component = targetObject.GetComponent<Renderer>();
        if (null == component)
        {
            errorInfo = "没有Render组件";
            return false;
        }
        Material material = component.get_sharedMaterial();
        if (null == material)
        {
            errorInfo = "没有Material";
            return false;
        }
        if (!string.IsNullOrEmpty(this.m_materialNamePattern.namePattern) && !base.isWildCardMatch(material.get_name(), this.m_materialNamePattern.namePattern, this.m_materialNamePattern.ignoreCase))
        {
            errorInfo = string.Format("材质名称不符合规范,要求为{0}({1})，实际为{2}", this.m_materialNamePattern.namePattern, this.m_materialNamePattern.IgnoreCaseStr, material.get_name());
            return false;
        }
        if ((this.m_shaderNamePatterns != null) && (this.m_shaderNamePatterns.Length > 0))
        {
            bool flag = false;
            string targetString = material.get_shader().get_name();
            foreach (TemplateMarkerBase.NamePattern pattern in this.m_shaderNamePatterns)
            {
                flag |= string.IsNullOrEmpty(pattern.namePattern) || base.isWildCardMatch(targetString, pattern.namePattern, pattern.ignoreCase);
                if (flag)
                {
                    break;
                }
            }
            if (!flag)
            {
                string str2 = string.Empty;
                foreach (TemplateMarkerBase.NamePattern pattern2 in this.m_shaderNamePatterns)
                {
                    str2 = str2 + string.Format("{0}({1})或", pattern2.namePattern, pattern2.IgnoreCaseStr);
                }
                char[] trimChars = new char[] { '或' };
                str2 = str2.TrimEnd(trimChars);
                errorInfo = string.Format("Shader名称不符合规范,要求为{0}，实际为{1}", str2, targetString);
                return false;
            }
        }
        if ((this.m_shaderCheckParams != null) && (this.m_shaderCheckParams.Length > 0))
        {
            string[] strArray = material.get_shaderKeywords();
            foreach (ShaderCheckParam param in this.m_shaderCheckParams)
            {
                if (!string.IsNullOrEmpty(param.m_paramNameInShader))
                {
                    if (param.m_Enable)
                    {
                        bool flag2 = false;
                        foreach (string str3 in strArray)
                        {
                            if (str3 == param.m_paramNameInShader)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                        if (!flag2)
                        {
                            errorInfo = string.Format("Shader参数勾选不符合规范,要求为：必须勾上{0}，实际为：没有勾上{0}", param.m_paramNameInShader);
                            return false;
                        }
                    }
                    else
                    {
                        bool flag3 = true;
                        foreach (string str4 in strArray)
                        {
                            if (str4 == param.m_paramNameInShader)
                            {
                                flag3 = false;
                                break;
                            }
                        }
                        if (!flag3)
                        {
                            errorInfo = string.Format("Shader参数勾选不符合规范,要求为：不得勾上{0}，实际为：勾上了{0}", param.m_paramNameInShader);
                            return false;
                        }
                    }
                }
            }
        }
        if (this.m_texCheckParams != null)
        {
            foreach (TextureCheckParam param2 in this.m_texCheckParams)
            {
                if ((param2 != null) && !string.IsNullOrEmpty(param2.m_texNameInShader))
                {
                    Texture texture = null;
                    if (material.HasProperty(param2.m_texNameInShader))
                    {
                        texture = material.GetTexture(param2.m_texNameInShader);
                    }
                    if (texture == null)
                    {
                        errorInfo = string.Format("材质{0}上{1}槽没有挂贴图", material.get_name(), param2.m_texNameInShader);
                        return false;
                    }
                    if (!base.isWildCardMatch(texture.get_name(), param2.m_textureNamePattern.namePattern, param2.m_textureNamePattern.ignoreCase))
                    {
                        errorInfo = string.Format("贴图名称不符合规范，要求为：{0}({1})，实际为：{2}", param2.m_textureNamePattern.namePattern, param2.m_textureNamePattern.IgnoreCaseStr, texture.get_name());
                        return false;
                    }
                    if ((param2.m_texSize > 0) && ((texture.get_width() > param2.m_texSize) || (texture.get_height() > param2.m_texSize)))
                    {
                        object[] args = new object[] { material.get_name(), texture.get_name(), param2.m_texSize, texture.get_width(), texture.get_height() };
                        errorInfo = string.Format("材质{0}上的贴图{1}尺寸超标，要求为：{2}x{2}，实际贴图为：{3}x{4}", args);
                        return false;
                    }
                }
            }
        }
        return true;
    }

    [Serializable]
    public class ShaderCheckParam
    {
        [Tooltip("该参数是否需要勾上")]
        public bool m_Enable;
        [Tooltip("shader参数的名字")]
        public string m_paramNameInShader;
    }

    [Serializable]
    public class TextureCheckParam
    {
        [Tooltip("贴图是否允许带alpha")]
        public bool m_Alpha;
        [Tooltip("贴图在shader中对应的名字")]
        public string m_texNameInShader;
        [Tooltip("贴图尺寸,填0表示不检查")]
        public int m_texSize;
        [Tooltip("贴图的命名规则")]
        public TemplateMarkerBase.NamePattern m_textureNamePattern;
    }
}

