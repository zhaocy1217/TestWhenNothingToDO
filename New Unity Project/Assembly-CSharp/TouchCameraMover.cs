using System;
using UnityEngine;

public class TouchCameraMover : MonoBehaviour
{
    private int ScrollTouchID = -1;

    private void Update()
    {
        foreach (Touch touch in Input.get_touches())
        {
            if ((touch.get_phase() == null) && (this.ScrollTouchID == -1))
            {
                this.ScrollTouchID = touch.get_fingerId();
            }
            if ((touch.get_phase() == 3) || (touch.get_phase() == 4))
            {
                this.ScrollTouchID = -1;
            }
            if ((touch.get_phase() == 1) && (touch.get_fingerId() == this.ScrollTouchID))
            {
                Vector3 vector = Camera.get_main().get_transform().get_position();
                Camera.get_main().get_transform().set_position(new Vector3(vector.x + (touch.get_deltaPosition().x * 0.2f), vector.y, vector.z + (touch.get_deltaPosition().y * 0.2f)));
            }
        }
    }
}

