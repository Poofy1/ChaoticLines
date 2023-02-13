using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorSwap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Chaos mainEvents;
    public SaveColor saveColor;
    public Transform colorCursor;
    public GameObject obj;

    public Image colorView;

    private bool held;
    private bool inside;
    private bool active;

    public void Update()
    {
        if (active)
        {
            colorCursor.position = Input.mousePosition;
            colorView.color = mainEvents.GetColor();
        }
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        held = true;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        held = false;
        if (!inside)
        {
            HideColorPicker();
        }
        saveColor.NewColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inside = true;
        Cursor.visible = false;
        active = true;
        obj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inside = false;

        if (!held)
        {
            HideColorPicker();
        }
        
    }


    private void HideColorPicker()
    {
        Cursor.visible = true;
        active = false;
        obj.SetActive(false);
    }
}
