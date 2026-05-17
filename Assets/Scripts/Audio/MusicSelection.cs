using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class MenuMusic : MonoBehaviour
{
    [field: Header("Scene Music")]

    [field: SerializeField] public EventReference sound { get; private set; }

    public EventInstance musicEventInstance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicEventInstance = RuntimeManager.CreateInstance(sound);
        musicEventInstance.start();
    }

    private void OnDestroy() 
    {
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        musicEventInstance.release();
    }
}