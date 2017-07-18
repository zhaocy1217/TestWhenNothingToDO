using System;
using UnityEngine;

[ComponentTypeSerializer(typeof(Camera))]
public class UnityCameraSerializer : ICustomizedComponentSerializer
{
    private const string XML_ATTR_CLEAR_FLAGS = "clear_flags";
    private const string XML_ATTR_CULLING_MASK = "cullingMask";
    private const string XML_ATTR_DEPTH = "depth";
    private const string XML_ATTR_FAR_PLANE = "farClipPlane";
    private const string XML_ATTR_FOV = "fieldOfView";
    private const string XML_ATTR_HDR = "hdr";
    private const string XML_ATTR_NEAR_PLANE = "nearClipPlane";
    private const string XML_ATTR_OC = "useOcclusionCulling";
    private const string XML_ATTR_PIXEL_RECT = "pixelRect";
    private const string XML_ATTR_RECT = "rect";
    private const string XML_ATTR_RENDER_PATH = "renderingPath";

    public void ComponentDeserialize(Component cmp, BinaryNode node)
    {
        Camera camera = cmp;
        camera.set_clearFlags((CameraClearFlags) int.Parse(GameSerializer.GetNodeAttr(node, "clear_flags")));
        camera.set_cullingMask(int.Parse(GameSerializer.GetNodeAttr(node, "cullingMask")));
        camera.set_fieldOfView(float.Parse(GameSerializer.GetNodeAttr(node, "fieldOfView")));
        camera.set_nearClipPlane(float.Parse(GameSerializer.GetNodeAttr(node, "nearClipPlane")));
        camera.set_farClipPlane(float.Parse(GameSerializer.GetNodeAttr(node, "farClipPlane")));
        camera.set_pixelRect(UnityBasetypeSerializer.StringToRect(GameSerializer.GetNodeAttr(node, "pixelRect")));
        camera.set_rect(UnityBasetypeSerializer.StringToRect(GameSerializer.GetNodeAttr(node, "rect")));
        camera.set_depth(float.Parse(GameSerializer.GetNodeAttr(node, "depth")));
        camera.set_renderingPath((RenderingPath) int.Parse(GameSerializer.GetNodeAttr(node, "renderingPath")));
        camera.set_useOcclusionCulling(bool.Parse(GameSerializer.GetNodeAttr(node, "useOcclusionCulling")));
        camera.set_hdr(bool.Parse(GameSerializer.GetNodeAttr(node, "hdr")));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        Camera camera = cmp;
        Camera camera2 = cmpPrefab;
        return ((((((camera.get_clearFlags() == camera2.get_clearFlags()) && (camera.get_cullingMask() == camera2.get_cullingMask())) && ((camera.get_fieldOfView() == camera2.get_fieldOfView()) && (camera.get_nearClipPlane() == camera2.get_nearClipPlane()))) && (((camera.get_farClipPlane() == camera2.get_farClipPlane()) && (camera.get_pixelRect() == camera2.get_pixelRect())) && ((camera.get_rect() == camera2.get_rect()) && (camera.get_depth() == camera2.get_depth())))) && ((camera.get_renderingPath() == camera2.get_renderingPath()) && (camera.get_useOcclusionCulling() == camera2.get_useOcclusionCulling()))) && (camera.get_hdr() == camera2.get_hdr()));
    }
}

