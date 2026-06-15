using System.Collections;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace HoldMyBeer.Audio
{
    public class MusicSelection : MonoBehaviour
    {
        [field: Header("Scene Music")]
        [field: SerializeField] public EventReference sound { get; private set; }

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1.5f;

        private EventInstance musicEventInstance;
        private Coroutine fadeCoroutine;

        private bool musicActive = true;
        private float currentVolume = 1f;

        private void Awake()
        {
            musicEventInstance = RuntimeManager.CreateInstance(sound);
        }

        private void Start()
        {
            // Music starts immediately when the level starts.
            musicEventInstance.start();
        }

        public void StopMusic()
        {
            if (musicEventInstance.isValid())
            {
                musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicEventInstance.release();
            }
        }

        /// <summary>
        /// A method that checks the state of the background music, when the player enters the collider, and uses the appropriate coroutine.
        /// </summary>
        public void HandleMusic()
        {
            if (!musicEventInstance.isValid()) return;

            musicActive = !musicActive; // Toggles music state

            if (fadeCoroutine != null) // Checks if a fade is already happening. If yes, it stops it.
            {
                StopCoroutine(fadeCoroutine);
            }

            if (musicActive) // If music state is true, it starts fading in.
            {
                fadeCoroutine = StartCoroutine(FadeIn());
            }

            else // Otherwise it fades out and pauses
            {
                fadeCoroutine = StartCoroutine(FadeOutAndPause());
            }
        }

        /// <summary>
        /// A coroutine that unpauses and fades in the background music, when it's paused.
        /// </summary>
        private IEnumerator FadeIn()
        {
            // Unpause first so the music can be heard while fading in.
            musicEventInstance.setPaused(false);

            float startVolume = currentVolume;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime; // Changes the time each frame.

                currentVolume = Mathf.Lerp(startVolume, 1f, time / fadeDuration); // Starts setting current volume to max volume (1).
                musicEventInstance.setVolume(currentVolume);

                yield return null; // Wait until next frame.
            }

            currentVolume = 1f;
            musicEventInstance.setVolume(currentVolume);

            fadeCoroutine = null; // Clears Coroutine
        }

        /// <summary>
        /// A coroutine that fades out the background music and then, pauses it.
        /// </summary>
        private IEnumerator FadeOutAndPause()
        {
            float startVolume = currentVolume;
            float time = 0f;

            while (time < fadeDuration)
            {
                time += Time.deltaTime; // Changes the time each frame.

                currentVolume = Mathf.Lerp(startVolume, 0f, time / fadeDuration); // Starts setting current volume minimum volume (0).
                musicEventInstance.setVolume(currentVolume);

                yield return null; // Wait until next frame.
            }

            currentVolume = 0f;
            musicEventInstance.setVolume(currentVolume);

            // Pause only after the fade-out is finished.
            musicEventInstance.setPaused(true);

            fadeCoroutine = null; // Clears Coroutine
        }

        private void OnDestroy()
        {
            if (musicEventInstance.isValid())
            {
                musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                musicEventInstance.release();
            }
        }
    }
}