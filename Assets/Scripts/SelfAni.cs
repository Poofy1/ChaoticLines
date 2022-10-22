using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelfAni : MonoBehaviour
{
    public float time;

    public ButtonAnimate buttonOff;
    public Transform min;
    public Transform max;

    public bool open;

    //When Clicked
    public void Clicked()
    {
        if (open) LeanTween.moveLocalX(gameObject, min.localPosition.x, time).setEaseInOutCubic();
        else LeanTween.moveLocalX(gameObject, max.localPosition.x, time).setEaseInOutCubic();
        open = !open;
        if(buttonOff != null) buttonOff.active = !buttonOff.active;
    }





}
