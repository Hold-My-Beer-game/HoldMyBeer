using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using Unity.Properties;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]
    public float MasterVolume = 1.0f;
    [Range(0, 1)]
    public float MusicVolume = 1.0f;
    [Range(0, 1)]
    public float SFXVolume = 1.0f;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private EventInstance musicEventInstance;
    private EventInstance ambienceEventInstance;
    public static AudioManager instance { get; private set; }

    private void Awake() 
    {
        if (instance != null) 
        {
            Debug.LogError("Found more than one Audio Managers on the scene");
        }
        instance = this;

        eventInstances = new List<EventInstance>(); // Create new event Instances variable list
        eventEmitters = new List<StudioEventEmitter>(); // Create new event Emitters Instances variable list

        masterBus = RuntimeManager.GetBus("bus:/"); // Initialize buses for audio 
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start() 
    {
        InitializeAmbience(SFXEvents.instance.WindAmbience);
    }

    private void Update() 
    {
        masterBus.setVolume(MasterVolume); // Update master, music and sfx volume in case they were changed in the inspector
        musicBus.setVolume(MusicVolume);
        sfxBus.setVolume(SFXVolume);
    }

    /// <summary>
    /// Method to set a local parameter in an FMOD Event, through its Event Instance. Other events that share the same parameter will not be affected.
    /// </summary>
    public void SetParameter(EventInstance eventName, string parameterName, float parameterValue) 
    {
        eventName.setParameterByName(parameterName, parameterValue);
    }

    /// <summary>
    /// Method to set a global parameter in FMOD, that impacts all events that have that parameter.
    /// </summary>
    public void SetGlobalParameter(string parameterName, float parameterValue) 
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
    }

    private void InitializeMusic(EventReference musicRef) 
    {
        musicEventInstance = CreateEventInstance(musicRef);
        musicEventInstance.start();
    }

    private void InitializeAmbience(EventReference ambRef) 
    {
        ambienceEventInstance = CreateEventInstance(ambRef);
        ambienceEventInstance.start();
    }

    /// <summary>
    /// Method to play one shot sound events, such as 2D and 3D actions. The event plays once.
    /// </summary>
    public void PlayOneShotEvent(EventReference sound, Vector3 pos) 
    {
        RuntimeManager.PlayOneShot(sound, pos);
    }

    /// <summary>
    /// Method to create a 2D or 3D timeline event instance. After being created, they need to be started and stopped manually.
    /// </summary>
    public EventInstance CreateEventInstance (EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance); // Add the event instance that was created to the list queue for clean up
        return eventInstance;
    }

    public StudioEventEmitter CreateEventEmitter(EventReference eventReference, GameObject emitterObj) 
    { 
        StudioEventEmitter emitter = emitterObj.GetComponent<StudioEventEmitter>(); // Get the emitter off the Game Object
        emitter.EventReference = eventReference; // Overwrite the emitters reference that is passed to this method
        eventEmitters.Add(emitter); // Add the emitter to the list queue for clean up
        return emitter;
    }

    private void CleanUp() 
    { 
        // Stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances) 
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        // Stop all of the event emitters on this list, because they may hang around in other scenes
        foreach (StudioEventEmitter emitter in eventEmitters) 
        { 
            emitter.Stop(); 
        }
        // Stop the background music and ambience
        musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ambienceEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void OnDestroy() 
    {
        CleanUp();
    }
}