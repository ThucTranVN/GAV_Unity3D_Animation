using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public float jumpSpeed;
    public float jumpPeriod;

    private CharacterController characterController;
    private Animator animator;
    private float ySpeed;
    private float? lastGroundedTime; //? = CheckNull
    private float? jumpPressdTime;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");  

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput);
        float inputMagnitude = Mathf.Clamp01(moveDirection.magnitude);

        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            inputMagnitude /= 2;
        }
        animator.SetFloat("Input", inputMagnitude, 0.05f, Time.deltaTime);

        moveDirection.Normalize();

        ySpeed += Physics.gravity.y * Time.deltaTime;

        if (characterController.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpPressdTime = Time.time;
        }

        if(Time.time - lastGroundedTime <= jumpPeriod)
        {
            ySpeed = -0.5f;
            if(Time.time - jumpPressdTime <= jumpPeriod)
            {
                ySpeed = jumpSpeed;
                jumpPressdTime = null;
                lastGroundedTime = null;
            }
        }

        
        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void OnAnimatorMove()
    {
        animator.SetFloat("MoveSpeed", moveSpeed);
        Vector3 velocity = animator.deltaPosition;
        velocity.y = ySpeed * Time.deltaTime;
        characterController.Move(velocity);
    }
}
