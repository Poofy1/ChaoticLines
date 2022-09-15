using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfAnimate : MonoBehaviour
{
    private Animator anim;
    private bool on;
    public bool block;
    public string[] names;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }



    public void Pressed(bool only1 = false)
    {
        if (!block)
        {
            if (only1) anim.Play(names[0]);
            else if (on) anim.Play(names[1]);
            else anim.Play(names[0]);
            on = !on;
        }
    }

    public void TurnOff()
    {
        if (on) anim.Play(names[1]);
        on = false;
    }
}
