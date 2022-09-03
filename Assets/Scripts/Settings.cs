using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public GameObject AAMenu;
    private bool vSyncOn;

    //FullScreen
    public void ToggleFS()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    //AA
    public void AA()
    {
        int setting = 0;
        if (AAMenu.GetComponent<TMP_Dropdown>().value == 3) setting = 8;
        else setting = AAMenu.GetComponent<TMP_Dropdown>().value * 2;

        QualitySettings.antiAliasing = setting;
    }

    //VSync
    public void VSync()
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
    }

    //Quit App
    public void Exit()
    {
        Application.Quit();
    }
}
