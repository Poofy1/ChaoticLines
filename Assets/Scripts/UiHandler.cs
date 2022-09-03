using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHandler : MonoBehaviour
{
    public GameObject[] targets;
    private bool[] on = new bool[3];
    private int speed;

    public GameObject[] buttons;

    public void SetSpeed(int input)
    {
        speed = input;
    }

    public void ButtonPressed(int window)
    {
        if (on[window])
        {
            targets[window].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(-speed * (Camera.main.aspect * .25f), 0, 0), ForceMode.Force);
            on[window] = false;
        }
        else
        {
            targets[window].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(speed * (Camera.main.aspect * .25f), 0, 0), ForceMode.Force);
            on[window] = true;
        }


    }

    public void Hovering(int index)
    {
        buttons[index].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 700 * (Camera.main.aspect * .25f), 0), ForceMode.Force);
    }

    public void NotHovering(int index)
    {
        buttons[index].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -700 * (Camera.main.aspect * .25f), 0), ForceMode.Force);
    }
}
