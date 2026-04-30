using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 0.2f;
    public Transform playerCamera;
    
    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float standingHeight = 2.0f;
    public float crouchSpeedMultiplier = 0.5f;
    public float cameraCrouchOffset = -0.5f;
    public float crouchSmoothness = 8f;
    

    private bool isCrouching = false;
    private float originalWalkSpeed;
    private float targetCameraY;
    private float originalCameraY;
    
    [Header("Sprint")]
    public float sprintMultiplier = 3.0f;
    private bool isSprinting = false;
    
    [Header("Sprint FOV")]
    public float sprintFOV = 80f;
    public float normalFOV = 60f;
    public float fovSmoothness = 8f;
    

    [Header("Footsteps")]
    public AudioSource footstepAudio;
    public float stepInterval = 0.5f;

    private CharacterController controller;
    private float yVelocity;
    private float xRotation = 0f;
    private Vector3 originalCenter;


    private Vector2 moveInput;
    private Vector2 lookInput;

    private float stepTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        
        originalWalkSpeed = walkSpeed;
        originalCameraY = playerCamera.localPosition.y;
        targetCameraY = originalCameraY;
        originalCenter = controller.center;
        
    }

    void Update()
    {
        Look();
        Move();
        HandleCrouchCamera();
        HandleFootsteps();
        HandleFOV();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnCrouch(InputValue value)
    {
        if (value.isPressed)
        {
            ToggleCrouch();
        }
    }

    public void OnSprint(InputValue value)
    {
        
        isSprinting = value.isPressed;
    }

    void Look()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        Vector3 move = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;


        if (controller.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;
        }

        yVelocity += gravity * Time.deltaTime;
        
        float currentSpeed = originalWalkSpeed;


        if (isCrouching)
        {
            currentSpeed *= crouchSpeedMultiplier;
        }


        if (isSprinting && !isCrouching)
        {
            currentSpeed *= sprintMultiplier;
        }
            

        Vector3 velocity = move * currentSpeed;
        velocity.y = yVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
    
    void HandleFOV()
    {
        Camera cam = playerCamera.GetComponent<Camera>();
        float targetFOV = isSprinting ? sprintFOV : normalFOV;

        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSmoothness);
    }


    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            controller.height = crouchHeight;
            controller.center = new Vector3(
                originalCenter.x,
                originalCenter.y - (standingHeight - crouchHeight) / 2f,
                originalCenter.z
            );

            targetCameraY = originalCameraY + cameraCrouchOffset;
        }
        else
        {
            controller.height = standingHeight;
            controller.center = originalCenter;

            targetCameraY = originalCameraY;
        }
    }

    void HandleCrouchCamera()
    {
        Vector3 camPos = playerCamera.localPosition;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * crouchSmoothness);
        playerCamera.localPosition = camPos;
    }

    void HandleFootsteps()
    {
        if (!controller.isGrounded) return;

        float movement = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);

        if (movement > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                footstepAudio.Play();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }
}