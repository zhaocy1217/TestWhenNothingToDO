using System;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public int rotateDirection = 1;
    public int rotateSpeed;

    private void Update()
    {
        base.get_gameObject().get_transform().Rotate((Vector3) (((Vector3.get_forward() * Time.get_deltaTime()) * this.rotateSpeed) * this.rotateDirection));
    }
}

