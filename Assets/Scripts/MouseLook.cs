using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Transform player;
    public Slider[] setting;
    public Text[] settingText;
    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation;
    public float mouseSensitivity;


    float offsetX;
    float offsetY;

    public bool locked = false;
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            //locked = true;
            Debug.Log("Application is focussed");
        }
        else
        {
            //locked = false;
            Debug.Log("Application lost focus");
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)) locked = false;

        if (locked)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * 10 * Time.deltaTime;
            mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * 10 * Time.deltaTime;
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

    public void GameClicked()
    {
        locked = true;
    }

    public void SensitivityChanged(int index)
    {
        mouseSensitivity = setting[index].value;
        if (index == 0) setting[1].value = setting[0].value;
        else setting[0].value = setting[1].value;
        for (int i = 0; i < 2; i++) settingText[i].text = mouseSensitivity.ToString("0");
    }
}
