using UnityEngine;
using UnityEngine.SceneManagement;
using HoldMyBeer.Input;

public class SceneTimerLoader : MonoBehaviour
{
    [Header("Timer Settings")]
    public float delay = 5f;

    [Header("Scene Settings")]
    public string sceneName;

    [Header("Mouse Settings")]
    public bool unlockMouseOnLoad = true; // Toggle in Inspector

    private bool hasLoaded = false;

    private void OnEnable() {
        PlayerInput.Instance.OnJumpKeyPressed += OnJumpPressed;
    }

    private void OnDisable() {
        PlayerInput.Instance.OnJumpKeyPressed -= OnJumpPressed;
    }

    private void Start()
    {
        PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.Player);
        PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.UI);
        
        Invoke(nameof(LoadScene), delay);
    }

    private void OnJumpPressed()
    {
        LoadScene();
    }

    private void LoadScene()
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