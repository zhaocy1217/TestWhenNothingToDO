using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDropScene : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Camera UICamera;
    public void OnDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().pivot.Set(0, 0);
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, UICamera, out pos);
        transform.position = transform.TransformPoint(pos);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        List<RaycastResult> rr = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, rr);
        Debug.LogError(rr.Count);


    }
}
