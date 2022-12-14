using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Chaos mainEvents;
    public Transform colorCursor;
    public GameObject obj;

    public Image colorView;

    private bool active;

    public void Update()
    {
        if (active)
        {
            colorCursor.position = Input.mousePosition;
            colorView.color = mainEvents.GetColor();
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.visible = false;
        active = true;
        obj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.visible = true;
        active = false;
        obj.SetActive(false);
    }
}
