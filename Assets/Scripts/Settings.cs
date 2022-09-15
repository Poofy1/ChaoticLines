using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    private bool vSyncOn;
    private bool safetyOn = true;
    public GameObject MMController;
    //Initial
    public Chaos mainEvents;
    public Camera cam;
    public AudioSource music;
    public AudioSource uiSound;
    public CanvasScaler[] huds;

    //Settings
    public GameObject[] AAMenu;
    public Toggle[] fullscreen;
    public Toggle[] vSync;
    public Toggle[] safety;
    public Slider[] musicSlider;
    public Text[] musicText;
    public Slider[] fovSlider;
    public Text[] fovText;
    public Slider[] hudSlider;
    public Text[] hudText;

    private void Start()
    {

    }

    //Volume
    public void VolumeChange(int index)
    {
        music.volume = musicSlider[index].value;
        uiSound.volume = musicSlider[index].value * .9f;
        if (index == 0) musicSlider[1].value = musicSlider[0].value;
        else musicSlider[0].value = musicSlider[1].value;
        for (int i = 0; i < 2; i++) musicText[i].text = music.volume.ToString("0.0");
    }

    //FOV
    public void fovChange(int index)
    {
        cam.fieldOfView = fovSlider[index].value;
        if (index == 0) fovSlider[1].value = fovSlider[0].value;
        else fovSlider[0].value = fovSlider[1].value;
        for (int i = 0; i < 2; i++) fovText[i].text = cam.fieldOfView.ToString("0");
    }

    //HUD Scale
    public void hudScale(int index)
    {
        for (int i = 0; i < 2; i++) hudText[i].text = hudSlider[index].value.ToString("0");
        
    }
    public void hudScaleApply(int index)
    {
        if (index == 0) hudSlider[1].value = hudSlider[0].value;
        else hudSlider[0].value = hudSlider[1].value;
        for (int i = 0; i < huds.Length; i++) huds[i].referenceResolution = new Vector2(hudSlider[0].value * 10, hudSlider[0].value * 10);
    }

    //FullScreen
    public void ToggleFS(int index)
    {
        Screen.fullScreen = !Screen.fullScreen;
        for (int i = 0; i < 2; i++) fullscreen[i].isOn = fullscreen[index].isOn;
    }

    //AA
    public void AA(int index)
    {
        int setting = 0;
        if (AAMenu[index].GetComponent<TMP_Dropdown>().value == 3) setting = 8;
        else setting = AAMenu[index].GetComponent<TMP_Dropdown>().value * 2;

        for (int i = 0; i < 2; i++) AAMenu[i].GetComponent<TMP_Dropdown>().value = AAMenu[index].GetComponent<TMP_Dropdown>().value;
        QualitySettings.antiAliasing = setting;
    }

    //VSync
    public void VSync(int index)
    {
        if (vSyncOn)
        {
            QualitySettings.vSyncCount = 0;
            vSyncOn = false;
        }
        else
        {
            QualitySettings.vSyncCount = 1;
            vSyncOn = true;
        }

        for (int i = 0; i < 2; i++) vSync[i].isOn = vSync[index].isOn;
    }

    //Divergence Safety
    public void Safety(int index)
    {
        mainEvents.safety = !mainEvents.safety;
        for (int i = 0; i < 2; i++) safety[i].isOn = safety[index].isOn;
    }

    //Quit App
    public void Exit()
    {
        Application.Quit();
    }


}
