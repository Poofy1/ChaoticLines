using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Refrences")]
    public CanvasScaler hud;

    [Header("Hovering Settings")]
    public bool transforming;
    public bool scaling;
    public float targetXoffset;
    public Vector2 targetScale;


    [Header("Time")]
    public float time;

    public float startingX;
    public bool active = true;

    private Vector3 startingScale;
    void Start()
    {
        startingScale = transform.localScale;
    }



    //Event System
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (active)
        {
            if (scaling) transform.LeanScale(targetScale, time);
            if (transforming) transform.LeanMoveLocalX(20, time);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (active)
        {
            if (scaling) transform.LeanScale(startingScale, time);
            if (transforming) transform.LeanMoveLocalX(-20, time);
        }
        
    }




}
