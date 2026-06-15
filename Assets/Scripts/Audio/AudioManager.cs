using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using HoldMyBeer.Zombies.Unity;
using System;

namespace HoldMyBeer.Audio {
    public class AudioManager : MonoBehaviour
    {
        [Header("Volume")]
        [Range(0, 1)]
        public float MasterVolume;
        [Range(0, 1)]
        public float MusicVolume;
        [Range(0, 1)]
        public float SFXVolume;

        private VCA masterVCA;
        private VCA musicVCA;
        private VCA sfxVCA;

        private List<EventInstance> eventInstances;
        private List<StudioEventEmitter> eventEmitters;
        private EventInstance musicEventInstance;
        private EventInstance ambienceEventInstance;
        public static AudioManager instance { get; private set; }

        private void Awake() 
        {
            if (instance != null && instance != this) 
            {
                Debug.LogError("Found more than one Audio Managers on the scene");
                Destroy(gameObject);
            }
            else {
                instance = this;
                DontDestroyOnLoad(gameObject);
                eventInstances = new List<EventInstance>(); // Create new event Instances variable list
                eventEmitters = new List<StudioEventEmitter>(); // Create new event Emitters Instances variable list

                masterVCA = RuntimeManager.GetVCA("VCA:/Master"); // Initialize VCAs for audio 
                musicVCA = RuntimeManager.GetVCA("VCA:/Music");
                sfxVCA = RuntimeManager.GetVCA("VCA:/SFX");
            }
        }

        private void Update() 
        {
            masterVCA.setVolume(Mathf.Clamp01(MasterVolume)); // Update master, music and sfx volume in case they were changed in the inspector
            musicVCA.setVolume(Mathf.Clamp01(MusicVolume));
            sfxVCA.setVolume(Mathf.Clamp01(SFXVolume));
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeAmbience(SFXEvents.instance.WindAmbience);
            ScreamerAudio();
            WalkerAudio();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            CleanUp();
        }

        private void WalkerAudio()
        {
            WalkerZombie[] walkers = FindObjectsByType<WalkerZombie>(FindObjectsSortMode.None);
            foreach (WalkerZombie walker in walkers) 
            {
                var walkerSteps = ConfigureEmitter(SFXEvents.instance.ZombieMovement, walker.gameObject);
                walkerSteps.Play();
                walker.OnStateChange += (gameObject, state) => 
                {
                    switch (state) 
                    {
                        case WalkerZombie.ZombieState.Idle:
                            walkerSteps.SetParameter("MovementStatus", (float) FMODParameters.MovementStatus.IDLE);
                            walkerSteps.SetParameter("MentalityStatus", (float) FMODParameters.MentalityStatus.PASSIVE);
                            break;
                        case WalkerZombie.ZombieState.Walk:
                            walkerSteps.SetParameter("MovementStatus", (float) FMODParameters.MovementStatus.WALKING);
                            walkerSteps.SetParameter("MentalityStatus", (float) FMODParameters.MentalityStatus.PASSIVE);
                            break;
                        case WalkerZombie.ZombieState.Aggro:
                            walkerSteps.SetParameter("MovementStatus", (float) FMODParameters.MovementStatus.WALKING);
                            walkerSteps.SetParameter("MentalityStatus", (float) FMODParameters.MentalityStatus.AGGRO);
                            break;
                        case WalkerZombie.ZombieState.Dead:
                            walkerSteps.AllowFadeout = false;
                            walkerSteps.Stop();
                            PlayOneShotEventObj(SFXEvents.instance.ZombieDeath, walker.gameObject);
                            break;
                    }
                };

                walker.OnTakeDamage += (gameObject) =>
                {
                    PlayOneShotEventObj(SFXEvents.instance.BulletHit, walker.gameObject);
                };
            }
        }

        private void ScreamerAudio()
        {
            // Finds all screamers in the scene.
            ScreamerZombie[] screamers = FindObjectsByType<ScreamerZombie>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
            );

            // The amount of times this will run is equal to the number of screamers in the game.
            // Each iteration belongs to a unique screamer.
            // Inside the foreach, run whatever you want to run per screamer zombie.
            // The switch case is also run per screamer.
            foreach (ScreamerZombie screamer in screamers) {
                // gameObject -> The screamer that raised the event.
                // state -> The state of the screamer.
                var screamerSteps = ConfigureEmitterMultiple(SFXEvents.instance.ScreamerRun, screamer.gameObject, 0);
                var screamerEat = ConfigureEmitterMultiple(SFXEvents.instance.ZombieEat, screamer.gameObject, 1);
                var screamerHiss = ConfigureEmitterMultiple(SFXEvents.instance.ScreamerZombie, screamer.gameObject, 2);

                screamerEat.AllowFadeout = true;
                screamerSteps.AllowFadeout = false;
                screamerHiss.AllowFadeout = false;

                screamer.OnStateChange += (gameObject, state) => {
                    switch (state) {
                        case ScreamerZombie.ZombieState.Eating:
                            screamerEat.Play();
                            break;
                        case ScreamerZombie.ZombieState.Alert:
                            screamerEat.Stop();
                            break;
                        case ScreamerZombie.ZombieState.Scream:
                            screamerHiss.Play();
                            break;
                        case ScreamerZombie.ZombieState.Run:
                            screamerSteps.Play();
                            break;
                        case ScreamerZombie.ZombieState.Dead:
                            screamerSteps.Stop();
                            screamerEat.Stop();
                            screamerHiss.Stop();
                            PlayOneShotEventObj(SFXEvents.instance.ZombieDeath, screamer.gameObject);
                            break;
                        default: throw new ArgumentOutOfRangeException();
                    }
                };

                screamer.OnTakeDamage += (gameObject) =>
                {
                    PlayOneShotEventObj(SFXEvents.instance.BulletHit, screamer.gameObject);
                };
            }
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
        /// Method to play one shot sound events, such as 2D and 3D actions using 3D position coordinates. The event plays once.
        /// </summary>
        public void PlayOneShotEvent(EventReference sound, Vector3 pos) 
        {
            RuntimeManager.PlayOneShot(sound, pos);
        }

        /// <summary>
        /// Method to play one shot sound events, such as 2D and 3D actions using a game object as reference. The event plays once.
        /// </summary>
        public void PlayOneShotEventObj(EventReference reference, GameObject obj)
        {
            RuntimeManager.PlayOneShotAttached(reference, obj);
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

        /// <summary>
        /// Modifies a StudioEventEmitter instance from an existing object. You start it with emitter.Play() and change parameters with emitter.SetParameter().
        /// </summary>
        /// <param name="eventReference">EventReference variable</param>
        /// <param name="emitterObj">GameObject variable of the object that plays the sound</param>
        /// <returns>StudioEventEmitter emitter and adds it for clean up.</returns>
        public StudioEventEmitter ConfigureEmitter(EventReference eventReference, GameObject emitterObj) 
        {
            if (emitterObj == null)
            {
                Debug.LogError("Emitter object is null.");
                return null;
            }

            StudioEventEmitter emitter = emitterObj.GetComponent<StudioEventEmitter>(); // Get the emitter off the Game Object

            if (emitter == null)
            {
                Debug.LogError($"No StudioEventEmitter found on {emitterObj.name}.");
                return null;
            }

            emitter.EventReference = eventReference; // Overwrite the emitters reference that is passed to this method
            eventEmitters.Add(emitter); // Add the emitter to the list queue for clean up
            return emitter;
        }

        /// <summary>
        /// Modifies a StudioEventEmitter instance from an existing object that has multiple StudioEventEmitter instances on it. You choose the desired instance with an index.
        /// Order is: 0, for highest on the inspector
        /// 1, for second highest etc.
        /// </summary>
        /// <param name="eventReference"></param>
        /// <param name="emitterObj"></param>
        /// <param name="emitterIndex"></param>
        /// <returns>Returns the StudioEventEmitter Object indicated by the index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public StudioEventEmitter ConfigureEmitterMultiple(EventReference eventReference, GameObject emitterObj, int emitterIndex)
        {
            if (emitterObj == null)
            {
                Debug.LogError("Emitter object is null.");
                return null;
            }

            StudioEventEmitter[] emitters = emitterObj.GetComponents<StudioEventEmitter>(); // Get the emitters off the Game Object

            if (emitterIndex < 0 || emitterIndex >= emitters.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(emitterIndex),
                    $"Emitter index {emitterIndex} is invalid on {emitterObj.name}. It has {emitters.Length} emitters."
                );
            }

            StudioEventEmitter emitter = emitters[emitterIndex];
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
            if (instance != this) return;
            CleanUp();
        }
    }
}