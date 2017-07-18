using System;
using UnityEngine;

public class UV_Rotate : MonoBehaviour
{
    public int rotateSpeed = 30;
    public Vector2 rotationCenter = Vector2.get_zero();
    public Texture texture;

    private void Start()
    {
        Material material = new Material(Shader.Find("Rotating Texture"));
        material.set_mainTexture(this.texture);
        base.GetComponent<Renderer>().set_material(material);
    }

    private void Update()
    {
        Quaternion quaternion = Quaternion.Euler(0f, 0f, Time.get_time() * this.rotateSpeed);
        Matrix4x4 matrixx = Matrix4x4.TRS(Vector3.get_zero(), quaternion, new Vector3(1f, 1f, 1f));
        Matrix4x4 matrixx2 = Matrix4x4.TRS(-this.rotationCenter, Quaternion.get_identity(), new Vector3(1f, 1f, 1f));
        Matrix4x4 matrixx3 = Matrix4x4.TRS(this.rotationCenter, Quaternion.get_identity(), new Vector3(1f, 1f, 1f));
        base.GetComponent<Renderer>().get_material().SetMatrix("_Rotation", (matrixx3 * matrixx) * matrixx2);
    }
}

