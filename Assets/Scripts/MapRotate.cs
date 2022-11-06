using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapRotate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IScrollHandler
{
    public Transform rotator;
    public Camera cam;

    private bool hovering;
    private bool dragging;
    public int maxZoomOut;
    private Vector2 offset;

    private void Update()
    {
        if (!dragging) return;

        var delta = (Vector2)Input.mousePosition - offset;
        offset = Input.mousePosition;

        delta = new Vector2(delta.y, -delta.x);

        rotator.Rotate(delta, Space.Self);
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!hovering) return;
        float delta = eventData.scrollDelta.y * 10000;

        if (cam.orthographicSize < 25000)
        {
            if(delta < 0) cam.orthographicSize -= delta * (cam.orthographicSize / 100000);
        }
        else if(cam.orthographicSize > maxZoomOut)
        {
            if(delta > 0) cam.orthographicSize -= delta * (cam.orthographicSize / 100000);
        }
        else cam.orthographicSize -= delta * (cam.orthographicSize / 100000);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hovering) return;
        dragging = true;
        offset = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
