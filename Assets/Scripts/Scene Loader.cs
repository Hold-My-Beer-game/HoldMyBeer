using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimerLoader : MonoBehaviour
{
    [Header("Timer Settings")]
    public float delay = 5f; // Time in seconds before loading

    [Header("Scene Settings")]
    public string sceneName; // Name of the scene to load

    private void Start()
    {
        Invoke(nameof(LoadScene), delay);
    }

    void LoadScene()
    {
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