using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class Settings : MonoBehaviour
{
    public bool vSyncOn;
    private bool safetyOn = true;
    //Initial
    public Chaos mainEvents;
    public Camera cam;
    public AudioSource music;
    public AudioSource uiSound;
    public CanvasScaler huds;

    public int AAsetting = 0;


    //Settings
    public GameObject AAMenu;
    public Toggle fullscreen;
    public Toggle vSync;
    public Toggle safety;
    public Slider musicSlider;
    public Text musicText;
    public Slider fovSlider;
    public Text fovText;
    public Slider hudSlider;
    public Text hudText;
    public Toggle tooltipButton;
    public TooltipManager toolManager;

    public void UpdateAll()
    {
        VolumeChange(0);
        fovChange(0);
        hudScale(0);
        hudScaleApply();
        ToggleFS(0);
        AA(0);
        Safety(0);
    }


    //Volume
    public void VolumeChange(int index)
    {
        music.volume = musicSlider.value;
        uiSound.volume = musicSlider.value;
        musicText.text = music.volume.ToString("0.00");
    }

    //FOV
    public void fovChange(int index)
    {
        cam.fieldOfView = fovSlider.value;
        fovText.text = cam.fieldOfView.ToString("0");
    }

    //HUD Scale
    public void hudScale(int index)
    {
        hudText.text = (hudSlider.value + 1).ToString("0.00");
    }
    public void hudScaleApply()
    {
        if(SteamUtils.IsRunningOnSteamDeck)
            huds.referenceResolution = new Vector2(0, (((hudSlider.value / 2) + 1) * 500) - 20);
        else
            huds.referenceResolution = new Vector2(0, ((hudSlider.value / 2) + 1) * 500);
    }

    //FullScreen
    public void ToggleFS(int index)
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen.isOn);
    }

    //AA
    public void AA(int index)
    {
        if (AAMenu.GetComponent<TMP_Dropdown>().value == 3) AAsetting = 8;
        else AAsetting = AAMenu.GetComponent<TMP_Dropdown>().value * 2;

        QualitySettings.antiAliasing = AAsetting;
    }

    //VSync
    public void VSync()
    {
        if (vSyncOn)
        {
            QualitySettings.vSyncCount = 0;
            vSyncOn = false;
            vSync.SetIsOnWithoutNotify(false);
        }
        else
        {
            QualitySettings.vSyncCount = 1;
            vSyncOn = true;
            vSync.SetIsOnWithoutNotify(true);
        }
    }

    //Tooltips
    public void Tooltip()
    {
        if (toolManager.active)
        {
            toolManager.active = false;
            tooltipButton.SetIsOnWithoutNotify(false);
        }
        else
        {
            toolManager.active = true;
            tooltipButton.SetIsOnWithoutNotify(true);
        }
        
    }

    //Divergence Safety
    public void Safety(int index)
    {
        mainEvents.safety = !mainEvents.safety;
    }

    //Quit App
    public void Exit()
    {
        Application.Quit();
    }




}
