using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;


public class F_CharacterController : MonoBehaviour
{
    //FROSTI CHARACTER CONTROLLER : made by Ben Franken

    //variables

    [Header("Base Settings")]
    public float speed = 5f;
    public float jumpHeight = 5f;
    public float rotationSpeed = 5f;
    public float gravityValue = 9.81f;

    [Header("Acceleration Settings")]
    public float baseSpeed;
    public float maxSpeed;
    public float acceleration;
    public float deceleration;

    //camera angles
    private float minVerticalAngle = -60f; //max distance you can look down
    private float maxVerticalAngle = 80f; //max distance you can look up
    private float currentX = 0f;
    private float currentY = 0f;

    [Header("Components")]
    public Transform Mpostion;
    public Camera playerCamera;
    public CharacterController playerController;
    private Vector3 targetPosition;
    public Vector3 jumpVelocity;


    //state of self... 
    private bool isRunning;
    private bool canJump;
    public bool isGrounded;

    private void Start()
    {
        speed = baseSpeed;

        targetPosition = transform.position;

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void MouseMovement()
    {
        // Left / Right Look
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);

        // Up / Down Look
        currentX -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentX = Mathf.Clamp(currentX, minVerticalAngle, maxVerticalAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(currentX, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        MouseMovement();
        isGrounded = playerController.isGrounded;

        //reads input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;

        speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);

        if (moveDirection != Vector3.zero)
        {
            targetPosition = transform.position + moveDirection * speed; // Update target based on input

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        }

        else
        {
            speed = Mathf.MoveTowards(speed, baseSpeed, deceleration * Time.deltaTime);
        }

        //jump mechanics

        //if (!playerController.isGrounded)
        //{
        //    moveDirection.y -= gravityValue * Time.deltaTime;
        //}

        //if (playerController.velocity.y < -1 && playerController.isGrounded)
        //{
        //    moveDirection.y = 0;
        //}
            

        //if (Input.GetButtonDown("Jump"))
        //{
        //    moveDirection.y = jumpHeight;
        //}


    }
}