using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


public class F_CharacterController : MonoBehaviour
{
    //FROSTI CHARACTER CONTROLLER : made by Ben Franken

    //variables
    public float speed = 5f;
    public float jumpHeight = 5f;
    public float rotationSpeed = 5f;
    public float distance = 0f;

    private float minVerticalAngle = -60f; //max distance you can look down
    private float maxVerticalAngle = 80f;

    //camera angles
    private float currentX = 0f;
    private float currentY = 0f;

    public Transform Mpostion;
    public Camera playerCamera;
    public CharacterController playerController;
    private Vector3 playerVelocity;

    private bool isRunning;
    private bool canJump;
    private bool isGrounded;

    [Header("Input Actions")]
    public InputActionReference moveAction; 
    public InputActionReference jumpAction;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    private void LateUpdate()
    {
        if (Mpostion == null) return;

        
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        

        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, - distance);
        Vector3 postion = rotation * negDistance + Mpostion.position;

        //transform.position = postion;
        transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
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
            transform.forward = move;
        }

        if (jumpAction.action.triggered && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f);
        }
        
        Vector3 finalMove = (move * speed) + (playerVelocity.y * Vector3.up);
        playerController.Move(finalMove * Time.deltaTime);
        
    }
}
