using UnityEngine;

namespace HoldMyBeer.UI {
    /// <summary>
    /// Editor/runtime state injector for HUD debugging.
    /// Directly mutates HUDState to validate UI rendering.
    /// </summary>
    [ExecuteAlways]
    internal class HUDTester : MonoBehaviour {
        private HUDState state;

        [Header("Vitals")]
        [Range(0f, 1f)] public float health = 1f;

        [Range(0f, 1f)] public float drunkness = 0f;

        [Header("Goal")]
        public string goalText = "Reach the objective";

        [Header("Interact")]
        public bool interactVisible;

        public string interactText = "Press E";

        [Header("Ammo")]
        public int ammo = 6;

        public void SetState(HUDState shared) {
            state = shared;
            Apply();
        }

        private void OnValidate() {
            Apply();
        }

        private void OnEnable() {
            Apply();
        }

        private void Apply() {
            if (state == null)
                return;

            state.Health = health;
            state.Drunkness = drunkness;

            state.GoalText = goalText;
            state.InteractVisible = interactVisible;
            state.InteractText = interactText;

            state.Ammo = ammo;

            state.Notify();
        }
    }
}