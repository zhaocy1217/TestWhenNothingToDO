using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ObjectNameMarker : TemplateMarkerBase
{
    public TemplateMarkerBase.NamePattern m_objNamePattern;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        string targetString = targetObject.get_name();
        bool flag = base.isWildCardMatch(targetString, this.m_objNamePattern.namePattern, this.m_objNamePattern.ignoreCase);
        if (!flag)
        {
            errorInfo = string.Format("prefab节点命名不符合规范，要求：{0}({1})，实际：{2}", this.m_objNamePattern.namePattern, this.m_objNamePattern.IgnoreCaseStr, targetString);
        }
        return flag;
    }
}

