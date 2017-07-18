using System;
using UnityEngine;

[ComponentTypeSerializer(typeof(RectTransform))]
public class RectTransformSerializer : ICustomizedComponentSerializer
{
    public void ComponentDeserialize(Component o, BinaryNode node)
    {
        RectTransform transform = o as RectTransform;
        transform.set_localScale(new Vector3(float.Parse(GameSerializer.GetAttribute(node, "SX")), float.Parse(GameSerializer.GetAttribute(node, "SY")), float.Parse(GameSerializer.GetAttribute(node, "SZ"))));
        transform.set_localRotation(new Quaternion(float.Parse(GameSerializer.GetAttribute(node, "RX")), float.Parse(GameSerializer.GetAttribute(node, "RY")), float.Parse(GameSerializer.GetAttribute(node, "RZ")), float.Parse(GameSerializer.GetAttribute(node, "RW"))));
        transform.set_anchorMin(new Vector2(float.Parse(GameSerializer.GetAttribute(node, "anchorMinX")), float.Parse(GameSerializer.GetAttribute(node, "anchorMinY"))));
        transform.set_anchorMax(new Vector2(float.Parse(GameSerializer.GetAttribute(node, "anchorMaxX")), float.Parse(GameSerializer.GetAttribute(node, "anchorMaxY"))));
        transform.set_offsetMin(new Vector2(float.Parse(GameSerializer.GetAttribute(node, "offsetMinX")), float.Parse(GameSerializer.GetAttribute(node, "offsetMinY"))));
        transform.set_offsetMax(new Vector2(float.Parse(GameSerializer.GetAttribute(node, "offsetMaxX")), float.Parse(GameSerializer.GetAttribute(node, "offsetMaxY"))));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        RectTransform transform = cmp as RectTransform;
        RectTransform transform2 = cmpPrefab as RectTransform;
        return (((((transform.get_localScale() == transform2.get_localScale()) && (transform.get_localRotation() == transform2.get_localRotation())) && ((transform.get_anchorMin() == transform2.get_anchorMin()) && (transform.get_anchorMax() == transform2.get_anchorMax()))) && (transform.get_offsetMin() == transform2.get_offsetMin())) && (transform.get_offsetMax() == transform2.get_offsetMax()));
    }
}

