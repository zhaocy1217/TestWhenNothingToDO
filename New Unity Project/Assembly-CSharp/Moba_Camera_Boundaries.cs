using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Moba_Camera_Boundaries
{
    public static string boundaryLayer = "mobaCameraBoundaryLayer";
    private static bool boundaryLayerExists = true;
    private static ListView<Moba_Camera_Boundary> cube_boundaries = new ListView<Moba_Camera_Boundary>();
    private static ListView<Moba_Camera_Boundary> sphere_boundaries = new ListView<Moba_Camera_Boundary>();

    public static bool AddBoundary(Moba_Camera_Boundary boundary, BoundaryType type)
    {
        if (boundary != null)
        {
            if (type == BoundaryType.cube)
            {
                cube_boundaries.Add(boundary);
                return true;
            }
            if (type == BoundaryType.sphere)
            {
                sphere_boundaries.Add(boundary);
                return true;
            }
        }
        return false;
    }

    private static Vector3 calBoxRelations(BoxCollider box, Vector3 point, bool containedToBox, out bool isPointInBox)
    {
        Vector3 vector = box.get_transform().get_position() + box.get_center();
        float num = (box.get_size().x / 2f) * box.get_transform().get_localScale().x;
        float num2 = (box.get_size().y / 2f) * box.get_transform().get_localScale().y;
        float num3 = (box.get_size().z / 2f) * box.get_transform().get_localScale().z;
        float num4 = Vector3.Dot(point - vector, box.get_transform().get_up());
        Vector3 vector2 = point + ((Vector3) (num4 * -box.get_transform().get_up()));
        float num5 = Vector3.Dot(vector2 - vector, box.get_transform().get_right());
        Vector3 vector3 = vector2 + ((Vector3) (num5 * -box.get_transform().get_right()));
        Vector3 vector4 = vector3 - vector;
        Vector3 vector5 = point - vector2;
        Vector3 vector6 = vector2 - vector3;
        float num6 = vector4.get_magnitude();
        float num7 = vector5.get_magnitude();
        float num8 = vector6.get_magnitude();
        isPointInBox = true;
        if (num6 > num3)
        {
            if (containedToBox)
            {
                num6 = num3;
            }
            isPointInBox = false;
        }
        if (num7 > num2)
        {
            if (containedToBox)
            {
                num7 = num2;
            }
            isPointInBox = false;
        }
        if (num8 > num)
        {
            if (containedToBox)
            {
                num8 = num;
            }
            isPointInBox = false;
        }
        num6 *= (Vector3.Dot(box.get_transform().get_forward(), vector4) < 0f) ? -1f : 1f;
        num7 *= (Vector3.Dot(box.get_transform().get_up(), vector5) < 0f) ? -1f : 1f;
        return new Vector3(num8 * ((Vector3.Dot(box.get_transform().get_right(), vector6) < 0f) ? -1f : 1f), num7, num6);
    }

    public static Moba_Camera_Boundary GetClosestBoundary(Vector3 point)
    {
        Moba_Camera_Boundary boundary = null;
        float num = 999999f;
        ListView<Moba_Camera_Boundary>.Enumerator enumerator = cube_boundaries.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Moba_Camera_Boundary current = enumerator.Current;
            if ((current != null) && current.isActive)
            {
                Vector3 vector = getClosestPointOnSurfaceBox(current.GetComponent<BoxCollider>(), point);
                float num2 = (point - vector).get_magnitude();
                if (num2 < num)
                {
                    boundary = current;
                    num = num2;
                }
            }
        }
        ListView<Moba_Camera_Boundary>.Enumerator enumerator2 = sphere_boundaries.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Moba_Camera_Boundary boundary3 = enumerator2.Current;
            if (boundary3.isActive)
            {
                SphereCollider component = boundary3.GetComponent<SphereCollider>();
                Vector3 vector2 = boundary3.get_transform().get_position() + component.get_center();
                float num3 = component.get_radius();
                Vector3 vector3 = point - vector2;
                Vector3 vector4 = vector2 + ((Vector3) (vector3.get_normalized() * num3));
                float num4 = (point - vector4).get_magnitude();
                if (num4 < num)
                {
                    boundary = boundary3;
                    num = num4;
                }
            }
        }
        return boundary;
    }

    public static Vector3 GetClosestPointOnBoundary(Moba_Camera_Boundary boundary, Vector3 point)
    {
        Vector3 vector = point;
        if (boundary.type == BoundaryType.cube)
        {
            return getClosestPointOnSurfaceBox(boundary.GetComponent<BoxCollider>(), point);
        }
        if (boundary.type == BoundaryType.sphere)
        {
            SphereCollider component = boundary.GetComponent<SphereCollider>();
            Vector3 vector2 = boundary.get_transform().get_position() + component.get_center();
            float num = component.get_radius();
            Vector3 vector3 = point - vector2;
            vector = vector2 + ((Vector3) (vector3.get_normalized() * num));
        }
        return vector;
    }

    private static Vector3 getClosestPointOnSurfaceBox(BoxCollider box, Vector3 point)
    {
        bool flag;
        Vector3 vector = calBoxRelations(box, point, true, out flag);
        return (Vector3) (((box.get_transform().get_position() + (box.get_transform().get_forward() * vector.z)) + (box.get_transform().get_right() * vector.x)) + (box.get_transform().get_up() * vector.y));
    }

    public static int GetNumberOfBoundaries()
    {
        return (cube_boundaries.Count + sphere_boundaries.Count);
    }

    public static bool isPointInBoundary(Vector3 point)
    {
        bool flag = false;
        ListView<Moba_Camera_Boundary>.Enumerator enumerator = cube_boundaries.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Moba_Camera_Boundary current = enumerator.Current;
            if (current.isActive)
            {
                BoxCollider component = current.GetComponent<BoxCollider>();
                if (component != null)
                {
                    bool flag2;
                    calBoxRelations(component, point, false, out flag2);
                    if (flag2)
                    {
                        flag = true;
                    }
                }
            }
        }
        ListView<Moba_Camera_Boundary>.Enumerator enumerator2 = sphere_boundaries.GetEnumerator();
        while (enumerator2.MoveNext())
        {
            Moba_Camera_Boundary boundary2 = enumerator2.Current;
            if (boundary2.isActive)
            {
                SphereCollider collider2 = boundary2.GetComponent<SphereCollider>();
                if (collider2 != null)
                {
                    Vector3 vector = (boundary2.get_transform().get_position() + collider2.get_center()) - point;
                    if (vector.get_magnitude() < collider2.get_radius())
                    {
                        flag = true;
                    }
                }
            }
        }
        return flag;
    }

    public static bool RemoveBoundary(Moba_Camera_Boundary boundary, BoundaryType type)
    {
        if (type == BoundaryType.cube)
        {
            return cube_boundaries.Remove(boundary);
        }
        return ((type == BoundaryType.sphere) && cube_boundaries.Remove(boundary));
    }

    public static void SetBoundaryLayerExist(bool value)
    {
        if (boundaryLayerExists)
        {
            boundaryLayerExists = false;
        }
    }

    public enum BoundaryType
    {
        cube,
        sphere,
        none
    }
}

