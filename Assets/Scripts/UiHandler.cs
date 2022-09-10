using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    //Initial
    public GameObject[] targets;
    private bool[] on;
    public Camera cam;
    private int speed;

    //Settings
    public GameObject[] buttons;
    public AudioSource music;
    public Slider musicSlider;
    public Text musicText;
    public Slider fovSlider;
    public Text fovText;

    private void Start()
    {
        on = new bool[buttons.Length];
    }


    //Menu Animation
    public void SetSpeed(int input)
    {
        speed = input * 1000;
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

    //Animated Buttons
    public void Hovering(int index)
    {
        buttons[index].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 1200 * (cam.aspect * .25f), 0), ForceMode.Force);
    }

    public void NotHovering(int index)
    {
        buttons[index].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, -1200 * (cam.aspect * .25f), 0), ForceMode.Force);
    }

    public void MainMenuHovering(int index)
    {
        buttons[index].transform.localScale += new Vector3(.1f, .1f, .1f);
    }

    public void MainMenuNotHovering(int index)
    {
        buttons[index].transform.localScale -= new Vector3(.1f, .1f, .1f);
    }

    //Settings
    public void VolumeChange()
    {
        music.volume = musicSlider.value;
        musicText.text = music.volume.ToString("0.0");
    }

    public void fovChange()
    {
        cam.fieldOfView = fovSlider.value;
        fovText.text = cam.fieldOfView.ToString("0");
    }
}
