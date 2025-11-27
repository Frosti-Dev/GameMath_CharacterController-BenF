using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
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
    public float crouchTransitionSpeed = 6f;

    [Header("Acceleration Settings")]
    public float baseSpeed;
    public float sprintMaxSpeed;
    public float crouchMaxSpeed;
    public float maxSpeed;
    public float acceleration;
    public float deceleration;


    //camera angles
    private float minVerticalAngle = -60f; //max distance you can look down
    private float maxVerticalAngle = 80f; //max distance you can look up
    private float currentX = 0f;
    private float currentY = 0f;

    [Header("Components")]
    public Transform mPostion;
    public Camera playerCamera;
    public GameObject cameraPosition;
    public CharacterController playerController;
    public Vector3 targetPosition;
    public Vector3 jumpVelocity;


    //state of self... 
    public bool isRunning;
    private bool canSprint;
    public bool isCrouching;
    public bool isGrounded;

    [Header("Heights")]
    public float normalHeight = 2.0f; 
    public Vector3 normalCenter = new Vector3(0, 1.0f, 0);
    public float crouchHeight = 1f;
    public Vector3 crouchCenter = new Vector3(0, 0.5f, 0);

    private float currentHeight;
    private Vector3 originalCenter;

    private void Start()
    {
        speed = baseSpeed;

        canSprint = true;

        targetPosition = transform.position;

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        originalCenter = playerController.center;
        currentHeight = normalHeight;

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
        Vector3 move = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        //speed states
        if (isRunning)
        {
            speed = Mathf.MoveTowards(speed, sprintMaxSpeed, acceleration * Time.deltaTime);
        }

        else if (isCrouching)
        {
            speed = Mathf.MoveTowards(speed, crouchMaxSpeed, acceleration * Time.deltaTime);
        }

        else
        {
            speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);
        }

        //move mechanics

        if (moveDirection != Vector3.zero)
        {
            targetPosition = transform.position + moveDirection * speed;



            transform.position = move;

        }

        else
        {
            targetPosition = Vector3.zero * Time.deltaTime;
            speed = Mathf.MoveTowards(speed, baseSpeed, deceleration * Time.deltaTime);
        }

        

        //sprint mechanics
        
        if (Input.GetKey(KeyCode.LeftShift) && canSprint)
        {
            isRunning = true;
        }

        else
        {
            isRunning = false;
        }

        //jump mechanics

        
        moveDirection.y -= gravityValue * Time.deltaTime;
        

        if (playerController.velocity.y < -1 && isGrounded)
        {
            moveDirection.y = -1;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            moveDirection.y = jumpHeight;
        }


        //crouch mechanics

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrouch();
        }


        if (Input.GetKeyUp(KeyCode.LeftControl) && isCrouching)
        {
            ToggleCrouch();
        }

        //final movements

        Vector3 finalMove = (moveDirection * speed) + (playerController.velocity.y * Vector3.up);
        playerController.Move(finalMove * Time.deltaTime);


    }

    void ToggleCrouch()
    {

        Vector3 crouchingCamera = new Vector3(cameraPosition.transform.position.x, (transform.position.y + 0.5f), cameraPosition.transform.position.z);
        Vector3 normalCamera = new Vector3(cameraPosition.transform.position.x, (transform.position.y + 1), cameraPosition.transform.position.z);


        isCrouching = !isCrouching;

        if (isCrouching)
        {
            canSprint = false;

            currentHeight = Mathf.Lerp(currentHeight, crouchHeight, crouchTransitionSpeed * Time.deltaTime);
            playerController.height = currentHeight;
            playerController.center = new Vector3(originalCenter.x, crouchCenter.y, originalCenter.z);
            cameraPosition.transform.position = crouchingCamera;


        }
        else
        {
            canSprint = true;

            currentHeight = Mathf.Lerp(currentHeight, normalHeight, crouchTransitionSpeed * Time.deltaTime);
            playerController.height = currentHeight;
            playerController.center = originalCenter;
            cameraPosition.transform.position = normalCamera;

        }
    }
}
