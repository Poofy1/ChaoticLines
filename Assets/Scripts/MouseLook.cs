using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform player;
    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation;
    public float mouseSensitivity = 100f;

    public bool locked = true;


    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            locked = true;
            Debug.Log("Application is focussed");
        }
        else
        {
            locked = false;
            Debug.Log("Application lost focus");
        }
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape)) locked = false;
        Debug.Log(Camera.main.aspect);

        if (locked)
        {
            Debug.Log("BROO");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yRotation += mouseX;
            xRotation -= mouseY;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            player.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
