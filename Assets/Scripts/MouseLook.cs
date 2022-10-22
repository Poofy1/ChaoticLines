using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Transform player;
    public Slider setting;
    public Text settingText;
    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation;
    public float mouseSensitivity;

    public bool active;


    float offsetX;
    float offsetY;

    public bool locked = false;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)) locked = false;

        if (locked && active)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
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
        mouseSensitivity = setting.value * .1f;
        for (int i = 0; i < 2; i++) settingText.text = (mouseSensitivity / 10).ToString("0.00");
    }
}
