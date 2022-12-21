using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailTemplate : MonoBehaviour
{
    public Transform polarTransform;
    public Transform trailTransform;
    public TrailRenderer trailSettings;


    //Positioning 
    public void PolarRotate(Vector3 rot, bool[] cordSystem)
    {
        if (cordSystem[0] == false) rot.z = 0;
        if (cordSystem[1] == false) rot.y = 0;
        if (cordSystem[2] == false) rot.x = 0;
        polarTransform.eulerAngles = rot;
    }

    public void TrailPos(Vector3 pos, bool[] cordSystem)
    {
        if (cordSystem[0] == true) pos.z = 0;
        if (cordSystem[1] == true) pos.y = 0;
        if (cordSystem[2] == true) pos.x = 0;
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