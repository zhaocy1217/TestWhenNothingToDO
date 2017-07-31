using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public struct RectInfo
{
    public int x, z, width, height;
}
public class Test0726 : MonoBehaviour {

    public int angle;
    public Vector3 offset;
    public GameObject target;
    public int x, z, width, height;
    public RectInfo r;
    public int fan_angle;
    public float radius;
	// Use this for initialization
	void OnEnable ()
    {

        r.x = x;
        r.z = z;
        r.width = width;
        r.height = height;
        Matrix4x4 tm = GetLocal2World();
        
        Debug.LogError(tm);
        Debug.LogError(transform.localToWorldMatrix);//.MultiplyPoint(Vector3.forward));
	}
    Matrix4x4 GetLocal2World()
    {
        Matrix4x4 tm = GetWorld2local();
        return tm.inverse;
    }
    Matrix4x4 GetWorld2local()
    {
        Matrix4x4 tm = new Matrix4x4();
        var _x = transform.rotation * Vector3.right;
        var _y = transform.rotation * Vector3.up;
        var _z = transform.rotation * Vector3.forward;
        var posi = Quaternion.Inverse(transform.rotation) * -transform.position;
        tm.SetRow(0, new Vector4(_x.x, _x.y, _x.z, posi.x) / transform.localScale.x);
        tm.SetRow(1, new Vector4(_y.x, _y.y, _y.z, posi.y) / transform.localScale.y);
        tm.SetRow(2, new Vector4(_z.x, _z.y, _z.z, posi.z) / transform.localScale.z);
        tm.SetRow(3, new Vector4(0, 0, 0, 1));
        return tm;
    }
    Vector3 Multiply(Vector3 lhs, Matrix4x4 rhs)
    {
        return new Vector3(
        (lhs.x * rhs[0, 0]) + (lhs.y * rhs[1, 0]) + (lhs.z * rhs[2, 0]),
        (lhs.x * rhs[0, 1]) + (lhs.y * rhs[1, 1]) + (lhs.z * rhs[2, 1]),
        (lhs.x * rhs[0, 2]) + (lhs.y * rhs[1, 2]) + (lhs.z * rhs[2, 2]));
    }
    // Update is called once per frame
    void Update ()
    {
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.up) * transform.rotation;
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(transform.position,q, Vector3.one);
        InFan(m, target.transform.position, Quaternion.AngleAxis(angle, Vector3.up) * transform.forward, offset, radius, fan_angle);
        InRect(m,r, target.transform.position);
    }
    bool InFan(Matrix4x4 m, Vector3 posi, Vector3 f, Vector3 offset, float radius, float angle)
    {
#if UNITY_EDITOR
        int zfactor = angle > 180 ? -1 : 1;
        float ta = Mathf.Sin(angle / 2);
        float tb = Mathf.Cos(angle / 2);
        float deg_angle = angle / 2 * Mathf.Deg2Rad;
        Vector3 v1 = new Vector3(-Mathf.Abs(Mathf.Sin(deg_angle)) * radius, 0, zfactor* Mathf.Abs(Mathf.Cos(deg_angle)) * radius) + offset;
        Vector3 v2 = new Vector3(Mathf.Abs(Mathf.Sin(deg_angle)) * radius, 0, zfactor * Mathf.Abs(Mathf.Cos(deg_angle)) * radius) + offset;
        v1 = m.MultiplyPoint(v1);
        v2 = m.MultiplyPoint(v2);
        Vector3 v_offset = m.MultiplyPoint(offset);
        Debug.DrawLine(v1, v_offset, Color.green);
        Debug.DrawLine(v2, v_offset, Color.green);
        Debug.DrawLine(v2, v1, Color.green);
#endif
        Vector3 p = m.inverse.MultiplyPoint(posi);
        float a = Vector3.Angle((p-offset), f);
        return a < fan_angle;
    }
    bool InRect(Matrix4x4 m, RectInfo r, Vector3 posi)
    {
       
#if UNITY_EDITOR
        Vector3 leftdown = new Vector3(r.x - r.width/2f, 0, r.z);
        Vector3 rightdown = new Vector3(r.x + r.width/2f, 0, r.z);
        Vector3 leftup = new Vector3(r.x - r.width/2f, 0, r.z + r.height);
        Vector3 rightup = new Vector3(r.x + r.width / 2f, 0, r.z + r.height);
        leftdown = m.MultiplyPoint(leftdown);
        leftup = m.MultiplyPoint(leftup);
        rightdown = m.MultiplyPoint(rightdown);
        rightup = m.MultiplyPoint(rightup);
        Debug.DrawLine(leftdown, leftup, Color.green);
        Debug.DrawLine(rightup, leftup, Color.green);
        Debug.DrawLine(rightdown, rightup, Color.green);
        Debug.DrawLine(leftdown, rightdown, Color.green);
#endif
        Vector3 p = m.inverse.MultiplyPoint(posi);
        if (p.x> r.x + r.width / 2f || p.x< r.x - r.width / 2f || p.z> r.z + r.height || p.z<r.z)
        {
            return false;
        }
        return true;
    }

}
