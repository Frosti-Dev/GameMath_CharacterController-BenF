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
    public float normalHeight = 1.0f; 
    public Vector3 normalCenter = new Vector3(0, 1.0f, 0);
    public float crouchHeight = 0.4f;
    public Vector3 crouchCenter = new Vector3(0, 0.5f, 0);


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

        if (!isGrounded)
        {
            moveDirection.y -= gravityValue * Time.deltaTime;
        }

        if (playerController.velocity.y < -1 && isGrounded)
        {
            moveDirection.y = 0;
        }

        


        if (Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
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

        
        //Vector3 finalMove = (move * speed) ; 
        //playerController.Move(finalMove * Time.deltaTime);

        ////(jumpVelocity.y * Vector3.up);
    }

    void ToggleCrouch()
    {
        Vector3 standingCamera = new Vector3(cameraPosition.transform.position.x, 2, cameraPosition.transform.position.z);
        Vector3 crouchCamera = new Vector3(cameraPosition.transform.position.x, 1, cameraPosition.transform.position.z);

        isCrouching = !isCrouching;

        if (isCrouching)
        {
            canSprint = false;
   
            cameraPosition.transform.position = crouchCamera;
            playerController.height = crouchHeight;
            playerController.center = crouchCenter;
            
        }
        else
        {
            canSprint = true;
            cameraPosition.transform.position = standingCamera;
            playerController.height = normalHeight;
            playerController.center = normalCenter;
                
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
