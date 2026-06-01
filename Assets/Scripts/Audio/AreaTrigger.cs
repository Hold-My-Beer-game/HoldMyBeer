using UnityEngine;

namespace HoldMyBeer.Audio
{
    public class AreaTrigger : MonoBehaviour
    {
        private MusicSelection music;

        private void Start()
        {
            music = FindFirstObjectByType<MusicSelection>();
        }

        private void OnTriggerEnter(Collider collider) {

            if (!collider.CompareTag("Player")) return;

            music.HandleMusic();
        }
    }
}