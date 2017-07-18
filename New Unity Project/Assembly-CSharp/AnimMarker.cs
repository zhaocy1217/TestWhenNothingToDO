using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AnimMarker : TemplateMarkerBase
{
    [Tooltip("贴图参数设置,填入该材质需要检测的贴图的张数")]
    public string[] m_animNameList;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        Animation component = targetObject.GetComponent<Animation>();
        if (null == component)
        {
            errorInfo = "没有Animation组件";
            return false;
        }
        if (this.m_animNameList != null)
        {
            bool flag = false;
            string str = string.Empty;
            foreach (string str2 in this.m_animNameList)
            {
                if (component.GetClip(str2) == null)
                {
                    flag = true;
                    str = str + str2 + ",";
                }
            }
            if (flag)
            {
                char[] trimChars = new char[] { ',' };
                errorInfo = "缺少必要的动画:" + str.TrimEnd(trimChars);
                return false;
            }
        }
        return true;
    }
}

