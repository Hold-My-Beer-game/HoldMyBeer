using FMODUnity;
using UnityEngine;

namespace HoldMyBeer.Audio
{
    public class ClubAudio : MonoBehaviour
    {
        private StudioEventEmitter emitter;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Soundtrack(MusicEvents.instance.DancingInThe90s, gameObject);
        }


        private void Soundtrack(EventReference reference, GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogError("Emitter object is null.");
            }

            emitter = AudioManager.instance.ConfigureEmitter(reference, obj);

            if (emitter == null)
            {
                Debug.LogError($"No StudioEventEmitter found on {obj.name}.");
            }

            emitter.EventReference = reference;
            emitter.Play();
            
        }

        private void OnDestroy() 
        {
            emitter.Stop();
        }
    }
}