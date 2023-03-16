using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{
    public Transform player;
    public Chaos mainEvents;
    public Slider setting;
    public Text settingText;
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

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Mouse1))
        {
            locked = false;
        }
            

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            GameClicked();
        }

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

            float xFinal = Mathf.Clamp(xRotation, -90f, 90f);
            float yFinal = yRotation;

            transform.rotation = Quaternion.Euler(xFinal, yFinal, 0);
            player.rotation = Quaternion.Euler(xFinal, yFinal, 0);
        }
        else
        {
            
            if (toggleOn)
            {
                Cursor.lockState = CursorLockMode.None;
                toggleOn = false;
                Cursor.visible = true;
            }
        }
    }

    public void GameClicked()
    {
        initialCamRot = transform.eulerAngles;
        yRotation = transform.eulerAngles.y;
        xRotation = (transform.eulerAngles.x > 180) ? transform.eulerAngles.x - 360 : transform.eulerAngles.x; ;
        locked = true;
    }

    public void SensitivityChanged(int index)
    {
        mouseSensitivity = setting.value * .1f;
        for (int i = 0; i < 2; i++) settingText.text = (mouseSensitivity / 10).ToString("0.00");
    }
}
