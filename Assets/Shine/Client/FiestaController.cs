using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class FiestaController : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed = 5f;
    [SerializeField]
    private float accelerateSpeed = 5f;
    private float worldRotationAngle = 0.0f;

    private Animator anim;

    private Vector2 movementInput;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        float horizontalAngle = movementInput.x == 1 ? 90 : movementInput.x == -1 ? 270 : 0;
        float verticalAngle = movementInput.y == -1 ? 180 : movementInput.y == 1 && movementInput.x < 0 ? 360 : 0;

        if (movementInput.x != 0 && movementInput.y != 0)
        {
            if (horizontalAngle > verticalAngle)
            {
                worldRotationAngle = verticalAngle + 45f;
            }
            else if (horizontalAngle < verticalAngle)
            {
                worldRotationAngle = horizontalAngle + 45f;
            }
            else
            {
                worldRotationAngle = 0.0f;
            }
        }
        else if (movementInput.x != 0)
        {
            worldRotationAngle = horizontalAngle;
        }
        else if (movementInput.y != 0)
        {
            worldRotationAngle = verticalAngle;
        }

        Quaternion targetRotation = Quaternion.AngleAxis(worldRotationAngle + 90, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime); //SmoothDamp?
    }

    private void HandleMovement()
    {
        float movementValue = Mathf.Lerp(
            anim.GetFloat("MovementAxis"), 
            Mathf.Abs(movementInput.magnitude), 
            accelerateSpeed * Time.deltaTime
        ); //SmoothDamp?

        anim.SetFloat("MovementAxis", movementValue);
    }
}
