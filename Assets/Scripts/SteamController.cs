using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        try
        {
            SteamClient.Init(2329480);
        }
        catch (System.Exception e)
        {
            // Couldn't init for some reason (steam is closed etc)
        }
    }

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }


    // Update is called once per frame
    void Update()
    {
        if (SteamClient.IsValid)
        {
            SteamClient.RunCallbacks();
        }
    }
}
