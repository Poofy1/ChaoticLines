using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTemplate : MonoBehaviour
{
    public Transform polarTransform;
    public Transform trailTransform;
    public TrailRenderer trailSettings;


    //Positioning 
    public void TrailPos(Vector3 pos)
    {
        trailTransform.localPosition = pos;
    }

    //Coloring
    public void SetColor(Color color)
    {
        trailSettings.startColor = color;
        color.a = .5f;
        trailSettings.endColor = color;
    }

    //Length
    public void SetLength(float len)
    {
        trailSettings.time = len;
    }

    //Thickness
    public void SetWidth(float thick)
    {
        trailSettings.startWidth = thick;
        trailSettings.endWidth = thick / 2;
    }
}