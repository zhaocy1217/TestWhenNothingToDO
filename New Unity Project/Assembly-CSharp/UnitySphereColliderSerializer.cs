using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(SphereCollider))]
public class UnitySphereColliderSerializer : ICustomizedComponentSerializer
{
    private const string XML_ATTR_CENTER = "center";
    private const string XML_ATTR_R = "radius";
    private const string XML_IS_TRIGGER = "trigger";

    public void ComponentDeserialize(Component cmp, BinaryNode node)
    {
        SphereCollider collider = cmp as SphereCollider;
        collider.set_center(UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center")));
        collider.set_radius(UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "radius")));
        collider.set_isTrigger(bool.Parse(GameSerializer.GetAttribute(node, "trigger")));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        SphereCollider collider = cmp as SphereCollider;
        SphereCollider collider2 = cmpPrefab as SphereCollider;
        return (((collider.get_center() == collider2.get_center()) && (collider.get_radius() == collider2.get_radius())) && (collider.get_isTrigger() == collider2.get_isTrigger()));
    }
}

