using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float jumpHeight;

    [SerializeField]
    private float gravityModifier;

    [SerializeField]
    private float jumpPeriod;

    [SerializeField]
    private float jumpHorizontalSpeed;

    [SerializeField]
    private Transform cameraTransform;

    private CharacterController characterController;
    private Animator animator;
    private float ySpeed;
    private float? lastGroundedTime; //? = CheckNull
    private float? jumpPressdTime;
    private bool isJumping;
    private bool isGrounded;


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
        moveDirection = Quaternion.AngleAxis(cameraTransform.transform.eulerAngles.y, Vector3.up) * moveDirection;
        moveDirection.Normalize();

        float gravity = Physics.gravity.y * gravityModifier;

        if(isJumping && ySpeed > 0 && Input.GetButton("Jump") == false)
        {
            gravity *= 2;
        }

        ySpeed += gravity * Time.deltaTime;

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
            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);

            if (Time.time - jumpPressdTime <= jumpPeriod)
            {
                ySpeed = Mathf.Sqrt(jumpHeight * -3 * gravity);
                animator.SetBool("IsJumping", true);
                isJumping = true;
                jumpPressdTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            isGrounded = false;

            if ((isJumping && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("IsFalling", true);
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

        if (isGrounded == false)
        {
            Vector3 velocity = moveDirection * inputMagnitude * jumpHorizontalSpeed;
            velocity.y = ySpeed;

            characterController.Move(velocity * Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        animator.SetFloat("MoveSpeed", moveSpeed);
        Vector3 velocity = animator.deltaPosition;
        velocity.y = ySpeed * Time.deltaTime;
        characterController.Move(velocity);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
