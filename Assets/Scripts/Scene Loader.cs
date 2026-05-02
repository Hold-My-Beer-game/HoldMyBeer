using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneTimerLoader : MonoBehaviour
{
    [Header("Timer Settings")]
    public float delay = 5f;

    [Header("Scene Settings")]
    public string sceneName;

    [Header("Mouse Settings")]
    public bool unlockMouseOnLoad = true; // Toggle in Inspector

    [Header("Input")]
    public InputActionReference jumpAction;

    private bool hasLoaded = false;

    private void OnEnable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.Enable();
            jumpAction.action.performed += OnJumpPressed;
        }
    }

    private void OnDisable()
    {
        if (jumpAction != null)
        {
            jumpAction.action.performed -= OnJumpPressed;
            jumpAction.action.Disable();
        }
    }

    private void Start()
    {
        Invoke(nameof(LoadScene), delay);
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        LoadScene();
    }

    void LoadScene()
    {
        if (hasLoaded) return;
        hasLoaded = true;

        // Apply mouse state based on toggle
        if (unlockMouseOnLoad)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set!");
        }
    }
}