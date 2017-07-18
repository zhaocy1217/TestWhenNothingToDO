using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class MeshMarker : TemplateMarkerBase
{
    [Tooltip("Mesh的命名规则")]
    public TemplateMarkerBase.NamePattern m_fbxNamePattern;
    [Tooltip("材质数，填0或负数表示不检测")]
    public int m_materialNum;
    [Tooltip("最大面数，填0或负数表示不检测")]
    public int m_maxTriangleNum;

    public override bool Check(GameObject targetObject, out string errorInfo)
    {
        errorInfo = string.Empty;
        MeshFilter component = targetObject.GetComponent<MeshFilter>();
        SkinnedMeshRenderer renderer = targetObject.GetComponent<SkinnedMeshRenderer>();
        if ((null == component) && (renderer == null))
        {
            errorInfo = "没有MeshFilter组件或者SkinnedMeshRender组件";
            return false;
        }
        Mesh mesh = null;
        if (null != component)
        {
            mesh = component.get_sharedMesh();
        }
        else
        {
            mesh = renderer.get_sharedMesh();
        }
        if (null == mesh)
        {
            errorInfo = "没有Mesh";
            return false;
        }
        if (!base.isWildCardMatch(mesh.get_name(), this.m_fbxNamePattern.namePattern, this.m_fbxNamePattern.ignoreCase))
        {
            errorInfo = string.Format("Mesh命名不符合规范，要求：{0}({1})，实际：{2}", this.m_fbxNamePattern.namePattern, this.m_fbxNamePattern.IgnoreCaseStr, mesh.get_name());
            return false;
        }
        if (this.m_maxTriangleNum > 0)
        {
            int num = mesh.get_triangles().Length / 3;
            if (num > this.m_maxTriangleNum)
            {
                errorInfo = string.Format("Mesh{0}最大面数超标，限定最大面数:{1}，实际面数:{2}", mesh.get_name(), this.m_maxTriangleNum, num);
                return false;
            }
        }
        if (this.m_materialNum > 0)
        {
            int length = targetObject.GetComponent<Renderer>().get_sharedMaterials().Length;
            if (length != this.m_materialNum)
            {
                errorInfo = string.Format("材质数量不符合标准，限定数量:{0}，实际数量:{1}", this.m_materialNum, length);
                return false;
            }
        }
        return true;
    }
}

