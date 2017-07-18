using System;
using UnityEngine;

public static class GeometryUtilityUser
{
    private static float[] ComVector = new float[4];
    private static float[] RootVector = new float[4];

    private static void CalcPlane(ref Plane InPlane, float InA, float InB, float InC, float InDistance)
    {
        Vector3 vector = new Vector3(InA, InB, InC);
        float num = 1f / ((float) Math.Sqrt((double) (((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z))));
        InPlane.set_normal(new Vector3(vector.x * num, vector.y * num, vector.z * num));
        InPlane.set_distance(InDistance * num);
    }

    public static void CalculateFrustumPlanes(Camera InCamera, ref Plane[] OutPlanes)
    {
        Matrix4x4 matrixx = InCamera.get_projectionMatrix();
        Matrix4x4 matrixx2 = InCamera.get_worldToCameraMatrix();
        Matrix4x4 matrixx3 = matrixx * matrixx2;
        RootVector[0] = matrixx3.get_Item(3, 0);
        RootVector[1] = matrixx3.get_Item(3, 1);
        RootVector[2] = matrixx3.get_Item(3, 2);
        RootVector[3] = matrixx3.get_Item(3, 3);
        ComVector[0] = matrixx3.get_Item(0, 0);
        ComVector[1] = matrixx3.get_Item(0, 1);
        ComVector[2] = matrixx3.get_Item(0, 2);
        ComVector[3] = matrixx3.get_Item(0, 3);
        CalcPlane(ref OutPlanes[0], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
        CalcPlane(ref OutPlanes[1], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);
        ComVector[0] = matrixx3.get_Item(1, 0);
        ComVector[1] = matrixx3.get_Item(1, 1);
        ComVector[2] = matrixx3.get_Item(1, 2);
        ComVector[3] = matrixx3.get_Item(1, 3);
        CalcPlane(ref OutPlanes[2], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
        CalcPlane(ref OutPlanes[3], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);
        ComVector[0] = matrixx3.get_Item(2, 0);
        ComVector[1] = matrixx3.get_Item(2, 1);
        ComVector[2] = matrixx3.get_Item(2, 2);
        ComVector[3] = matrixx3.get_Item(2, 3);
        CalcPlane(ref OutPlanes[4], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
        CalcPlane(ref OutPlanes[5], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);
    }

    private enum EPlaneSide
    {
        Left,
        Right,
        Bottom,
        Top,
        Near,
        Far
    }
}

