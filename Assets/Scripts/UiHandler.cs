using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    public GameObject[] targets;
    private bool[] on = new bool[4];
    private Camera cam;
    private int speed;

    public GameObject[] buttons;
    public AudioSource music;
    public Slider musicSlider;
    public Text musicText;

    private void Start()
    {
        cam = Camera.main;
    }

    public void SetSpeed(int input)
    {
        speed = input;
    }

    public void ButtonPressed(int window)
    {
        if (on[window])
        {
            targets[window].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(-speed * (cam.aspect * .25f), 0, 0), ForceMode.Force);
            on[window] = false;
        }
        else
        {
            targets[window].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(speed * (cam.aspect * .25f), 0, 0), ForceMode.Force);
            on[window] = true;
        }


    }

    public void Hovering(int index)
    {
        buttons[index].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 1200 * (cam.aspect * .25f), 0), ForceMode.Force);
    }

    public void NotHovering(int index)
    {
        buttons[index].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -1200 * (cam.aspect * .25f), 0), ForceMode.Force);
    }

    public void VolumeChange()
    {
        music.volume = musicSlider.value;
        musicText.text = music.volume.ToString("0.0");
    }
}
