using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneTimerLoader : MonoBehaviour
{
    [Header("Timer Settings")]
    public float delay = 5f;

    [Header("Scene Settings")]
    public string sceneName;

    [Header("Input")]
    public InputActionReference jumpAction; // Assign your Jump action here

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
        LoadScene(); // Skip timer if player presses jump
    }

    void LoadScene()
    {
        if (hasLoaded) return; // Prevent double loading
        hasLoaded = true;

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