using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseAnimation : MonoBehaviour
{
    private Animator anim;
    public string[] names;
    private Vector3 temp;
    private bool off;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void Pressed(int index)
    {
        if (!off) anim.Play(names[index]);
        if (index == 2) off = true;
    }

    public void ResetPos()
    {
        off = false;
    }
}
