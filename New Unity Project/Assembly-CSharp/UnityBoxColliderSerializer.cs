using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(BoxCollider))]
public class UnityBoxColliderSerializer : ICustomizedComponentSerializer
{
    private const string XML_ATTR_CENTER = "center";
    private const string XML_ATTR_SIZE = "size";
    private const string XML_IS_TRIGGER = "trigger";

    public void ComponentDeserialize(Component cmp, BinaryNode node)
    {
        BoxCollider collider = cmp as BoxCollider;
        collider.set_center(UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center")));
        collider.set_size(UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "size")));
        collider.set_isTrigger(bool.Parse(GameSerializer.GetAttribute(node, "trigger")));
    }

    public bool IsComponentSame(Component cmp, Component cmpPrefab)
    {
        BoxCollider collider = cmp as BoxCollider;
        BoxCollider collider2 = cmpPrefab as BoxCollider;
        return (((collider.get_center() == collider2.get_center()) && (collider.get_size() == collider2.get_size())) && (collider.get_isTrigger() == collider2.get_isTrigger()));
    }
}

