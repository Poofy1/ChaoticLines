using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager._instance.InitializeTooltip(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager._instance.HideTooltip();
    }
}
