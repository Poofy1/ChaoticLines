using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    private float moveSpeed;

    [Header("Keybinds")]
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Objects")]
    public Transform orientation;
    public Material crosshair;
    public MouseLook mouseLook;

    public float dis;
    public bool active;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;


    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!(EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() != null))
        {
            MyInput();
            SpeedControl();
        }

        float x = Mathf.Clamp(transform.localPosition.x, -2000000, 2000000);
        float y = Mathf.Clamp(transform.localPosition.y, -2000000, 2000000);
        float z = Mathf.Clamp(transform.localPosition.z, -2000000, 2000000);

        transform.localPosition = new Vector3(x,y,z);
    }

    private void FixedUpdate()
    {

        if (active) MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        dis = Vector3.Distance(new Vector3(0, 0, 0), transform.position);

        if (Input.GetKey(runKey)) {
            moveSpeed = (Math.Max(1, dis - 2) + walkSpeed) * sprintSpeed;
        }
        else
        {
            moveSpeed = Math.Max(1, dis - 2) + walkSpeed;
        }

        //Move up
        if (Input.GetKey(KeyCode.Space)) rb.AddForce(new Vector3(0, 1, 0) * moveSpeed * 3, ForceMode.Force);

        //Move down
        if (Input.GetKey(KeyCode.LeftControl)) rb.AddForce(new Vector3(0, -1, 0) * moveSpeed * 3, ForceMode.Force);

        crosshair.color = new Color(1, 1, 1, Math.Max(0, (float) Math.Pow(dis, 2) - 2));

        if (Input.GetKeyDown("r"))
        {
            //reset rotation
            mouseLook.transform.eulerAngles = Vector3.zero;
            mouseLook.player.eulerAngles = Vector3.zero;

            mouseLook.initialCamRot.x = 0;
            mouseLook.initialCamRot.y = 0;

            mouseLook.yRotation = 0;
            mouseLook.xRotation = 0;


            //reset position
            transform.position = new Vector3(0, 0, -1);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}