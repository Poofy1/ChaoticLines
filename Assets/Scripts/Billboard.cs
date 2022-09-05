using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private bool on = true;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    
    void LateUpdate()
    {
        if (on)
        {
            transform.LookAt(cam.transform);
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

    public void ToggleCrosshair()
    {
        on = !on;
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
