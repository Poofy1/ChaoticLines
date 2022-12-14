using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimate : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Refrences")]
    public CanvasScaler hud;
    public AudioSource uiSounds;
    public AudioClip audioSelect;
    public AudioClip audioPress;

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
            uiSounds.PlayOneShot(audioSelect, .15f);
            if (scaling) transform.LeanScale(targetScale, time);
            //if (transforming) transform.LeanMoveX(startingX + targetXoffset * (1000 / hud.referenceResolution.x), time).setEaseInOutCubic();
            if (transforming) LeanTween.moveX(gameObject, transform.position.x + 20, time).setEaseInOutCubic();
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (active)
        {
            uiSounds.PlayOneShot(audioSelect, .15f);
            if (scaling) transform.LeanScale(startingScale, time);
            if (transforming) LeanTween.moveX(gameObject, transform.position.x - 20, time).setEaseInOutCubic();
        }
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (active) uiSounds.PlayOneShot(audioPress, .3f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }


}
