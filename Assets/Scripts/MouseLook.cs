using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Transform player;
    public Slider setting;
    public Text settingText;
    public bool mouseSmooth;
    public float smoothing = 2.0f; // control the amount of smoothing
    private Vector2 smoothMouse = Vector2.zero; // stores the smoothed mouse input values
    public float mouseX;
    public float mouseY;
    public float xRotation;
    public float yRotation;
    public float mouseSensitivity;

    public bool active;

    public bool locked = false;

    private bool toggleOn;
    public Vector3 initialCamRot;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape)) locked = false;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            locked = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1)) locked = false;

        if (locked && active)
        {
            mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
            mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            toggleOn = true;

            yRotation += mouseX;
            xRotation -= mouseY;



            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation + initialCamRot.x, yRotation + initialCamRot.y, 0);
            player.rotation = Quaternion.Euler(xRotation + initialCamRot.x, yRotation + initialCamRot.y, 0);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            if (toggleOn)
            {
                toggleOn = false;
                Cursor.visible = true;
            }
        }
    }

    public void GameClicked()
    {
        initialCamRot = transform.eulerAngles;
        yRotation = 0;
        xRotation = 0;
        locked = true;
    }

    public void SensitivityChanged(int index)
    {
        mouseSensitivity = setting.value * .1f;
        for (int i = 0; i < 2; i++) settingText.text = (mouseSensitivity / 10).ToString("0.00");
    }
}
