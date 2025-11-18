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
    public float distance = 0f;

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
    private Vector3 playerVelocity;

    //state of self... 
    private bool isRunning;
    private bool canJump;
    private bool isGrounded;

    [Header("Input Actions")]
    public InputActionReference moveAction; 
    public InputActionReference jumpAction;

    private void Start()
    {
        speed = baseSpeed;

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
    }

    //private void LateUpdate()
    //{
    //    Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

    //    Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
    //    Vector3 position = rotation * negDistance + transform.position;

    //    transform.position = position;
    //    transform.rotation = rotation;
    //}

    private void MouseMovement()
    {
        // Left / Right Look
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * rotationSpeed, 0);

        // Up/Down Look
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + transform.position;

        transform.position = position;
        //transform.rotation = rotation; //breaks the y rotation but fixes x rotation?
    }

    // Update is called once per frame
    void Update()
    {
        MouseMovement();
        isGrounded = playerController.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        //reads input
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = Vector3.ClampMagnitude(move, 1f);

        if (move != Vector3.zero)
        {
            transform.position = move;
            //Vector3 target.transform.forward =  10 ;
            //transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }

        if (jumpAction.action.triggered && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f);
        }
        
        Vector3 finalMove = (move * speed) + (playerVelocity.y * Vector3.up);
        playerController.Move(finalMove * Time.deltaTime);
        
    }
}




/*
 * [SerializeField] Transform target;
    [SerializeField] float followRange;
 
    [SerializeField] float baseSpeed;
    [SerializeField] float maxSpeed;
    float speed;
 
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
 
    private void Start()
    {
        speed = baseSpeed;
    }
 
    void Update()
    {
        //distance between game objects position (gameobject that this script is on) & target Posiotion
        float distToTarget = Vector3.Distance(transform.position, target.position);
 
        if (distToTarget <= followRange)
        {
            transform.LookAt(target);
 
            speed = Mathf.MoveTowards(speed, maxSpeed, acceleration * Time.deltaTime);
 
            //Vector3.MoveTowards
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime); //1 is a magic number
        }
        else
        {
            speed = Mathf.MoveTowards(speed, baseSpeed, deceleration * Time.deltaTime);
            transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
 
        }
    }

*/