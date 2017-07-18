using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Test526 : MonoBehaviour,IDragHandler
{

    public Camera UICamera;
    public Camera MainCamera;
    public Image image1;
    public Image image2;
    private float ratio = 0.0078125f;
    private Vector3 uguiScreen = new Vector3(720, 1280, 10);
    private Vector3 MainScreen = new Vector3(Screen.width, Screen.height,10);
    // Use this for initialization
    void Start () {

    }
    public void OnDrag()
    {

    }
    bool _pressed = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        Debug.LogError("down "+eventData.clickTime);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.LogError("exit " + eventData.clickTime);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
        Debug.LogError("up " + eventData.clickTime);
    }

    void Update()
    {
        if (!_pressed)
            return;

        // DO SOMETHING HERE
    }
    private Vector3 Caculate(Vector3 localposi)
    {
        Vector3 localPosi = localposi;
        Vector3 screenPosi = Mul( Div(localPosi, uguiScreen), MainScreen) + MainScreen/2;
        var caculated_main_camera_viewPosi = MainCamera.ScreenToWorldPoint(screenPosi);
        return caculated_main_camera_viewPosi;
    }
    Vector3 Mul(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x*b.x, a.y*b.y, a.z*b.z);
    }
    Vector3 Div(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag drag");
    }

    // Update is called once per frame
}
