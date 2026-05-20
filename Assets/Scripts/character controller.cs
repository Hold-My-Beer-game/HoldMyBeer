using UnityEngine;
using UnityEngine.InputSystem;
using FMOD.Studio;

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

    // New: Separate collider references for crouch
    public Collider standingCollider;
    public Collider crouchingCollider;

    private bool isCrouching = false;
    private float originalWalkSpeed;
    private float targetCameraY;
    private float originalCameraY;

    [Header("Sprint")]
    public float sprintMultiplier = 3.0f;
    public bool sprintEnabled = true; // Toggle sprint on/off from inspector
    private bool isSprinting = false;

    [Header("Sprint FOV")]
    public float sprintFOV = 80f;
    public float normalFOV = 60f;
    public float fovSmoothness = 8f;


    //[Header("Footsteps")]
    //public AudioSource footstepAudio;
    //public float stepInterval = 0.5f;

    private CharacterController controller;
    private float yVelocity;
    private float xRotation = 0f;
    private Vector3 originalCenter;


    private Vector2 moveInput;
    private Vector2 lookInput;

    private float stepTimer;

    // audio
    private EventInstance playerFootsteps;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        originalWalkSpeed = walkSpeed;
        originalCameraY = playerCamera.localPosition.y;
        targetCameraY = originalCameraY;
        originalCenter = controller.center;

        // Initialize colliders based on starting state
        if (standingCollider != null && crouchingCollider != null)
        {
            standingCollider.enabled = true;
            crouchingCollider.enabled = false;
        }
        // Initialize the event instance for player footsteps audio
        playerFootsteps = AudioManager.instance.CreateEventInstance(SFXEvents.instance.PlayerSteps);
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
        // Only allow sprinting if sprint is enabled in inspector
        if (sprintEnabled)
        {
            // Sprint only while holding down the button
            isSprinting = value.isPressed && !isCrouching;
        }
        else
        {
            isSprinting = false;
        }
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


        if (isSprinting && !isCrouching && sprintEnabled)
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
            // Change CharacterController
            controller.height = crouchHeight;
            controller.center = new Vector3(
                originalCenter.x,
                originalCenter.y - (standingHeight - crouchHeight) / 2f,
                originalCenter.z
            );

            // Change colliders (if assigned)
            if (standingCollider != null && crouchingCollider != null)
            {
                standingCollider.enabled = false;
                crouchingCollider.enabled = true;
            }

            targetCameraY = originalCameraY + cameraCrouchOffset;
        }
        else
        {
            // Change CharacterController
            controller.height = standingHeight;
            controller.center = originalCenter;

            // Change colliders (if assigned)
            if (standingCollider != null && crouchingCollider != null)
            {
                standingCollider.enabled = true;
                crouchingCollider.enabled = false;
            }

            targetCameraY = originalCameraY;
        }

        // Stop sprinting when crouching
        if (isCrouching && isSprinting)
        {
            isSprinting = false;
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

        if (isCrouching) 
        {
            // If crouching decrease frequency of steps
            AudioManager.instance.SetParameter(playerFootsteps, "MovementStatus", (float) FMODParameters.MovementStatus.CROUCHING);
        }
        else 
        {
            // If not crouching, set original frequency of steps
            AudioManager.instance.SetParameter(playerFootsteps, "MovementStatus", (float) FMODParameters.MovementStatus.WALKING);
        }

        if (movement > 0.1f)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState); // Get the current state of the playerFootsteps event
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start(); // Play if there is movement and the event is not already playing
            }
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT); // Stop if no movement, but allow fade out for smoother audio transition
        }
    }
}