using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(CapsuleCollider))]
public class UnityCapsuleColliderrSerializer : ICustomizedComponentSerializer
{
    private const string XML_ATTR_CENTER = "center";
    private const string XML_ATTR_D = "dir";
    private const string XML_ATTR_H = "height";
    private const string XML_ATTR_R = "radius";
    private const string XML_IS_TRIGGER = "trigger";

    public void ComponentDeserialize(Component cmp, BinaryNode node)
    {
        CapsuleCollider collider = cmp as CapsuleCollider;
        collider.set_center(UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center")));
        collider.set_radius(UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "radius")));
        collider.set_height(UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "height")));
        collider.set_direction(UnityBasetypeSerializer.BytesToInt(GameSerializer.GetBinaryAttribute(node, "dir")));
        collider.set_isTrigger(bool.Parse(GameSerializer.GetAttribute(node, "trigger")));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        CapsuleCollider collider = cmp as CapsuleCollider;
        CapsuleCollider collider2 = cmpPrefab as CapsuleCollider;
        return ((((collider.get_center() == collider2.get_center()) && (collider.get_radius() == collider2.get_radius())) && ((collider.get_height() == collider2.get_height()) && (collider.get_direction() == collider2.get_direction()))) && (collider.get_isTrigger() == collider2.get_isTrigger()));
    }
}

