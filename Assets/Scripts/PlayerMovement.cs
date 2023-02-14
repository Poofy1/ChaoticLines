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
            moveSpeed = (Math.Max(0, dis - 3) + walkSpeed) * sprintSpeed;
        }
        else
        {
            moveSpeed = Math.Max(0, dis - 3) + walkSpeed;
        }

        crosshair.color = new Color(1, 1, 1, Math.Max(0, (float) Math.Pow(dis, 2) - 2));

        if (Input.GetKeyDown("r")) transform.position = new Vector3(0, 0, 0);
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