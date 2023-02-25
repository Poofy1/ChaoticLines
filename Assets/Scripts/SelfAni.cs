using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SelfAni : MonoBehaviour
{
    public float time;

    public Transform min;
    public Transform max;

    public bool open;

    //When Clicked
    public void Clicked()
    {
        if (open) LeanTween.moveLocalX(gameObject, min.localPosition.x, time).setEaseInOutCubic();
        else LeanTween.moveLocalX(gameObject, max.localPosition.x, time).setEaseInOutCubic();
        open = !open;
    }


    // Made for hotkey hints:
    public TextMeshProUGUI title;

    public void VerticalClicked()
    {
        if (open)
        {
            LeanTween.moveLocalY(gameObject, min.localPosition.y, time).setEaseInOutCubic();
            title.text = "Show Hotkeys";
        }
        else
        {
            LeanTween.moveLocalY(gameObject, max.localPosition.y, time).setEaseInOutCubic();
            title.text = "Hide Hotkeys";
        }
        open = !open;
    }


}
