using UnityEngine;

namespace HoldMyBeer.UI {
    /// <summary>
    /// Converts gameplay systems into HUD state mutations.
    /// </summary>
    internal class HUDController {
        private readonly HUDState state;

        // todo HUD interface
        private IPlayerStateRead playerStateRead;

        public void Init() {
            // view.Bind(state);

            if (playerStateRead == null) { return; }
            playerStateRead.OnHealthChange += SetHealth;
            playerStateRead.OnAlcoholChange += SetDrunkness;
            playerStateRead.OnGoalChange += SetGoal;
            playerStateRead.OnInteract += SetInteract;
            playerStateRead.OnAmmoChange += SetAmmo;
        }

        public HUDController(HUDState state, IPlayerStateRead playerStateReadRef) {
            this.state = state;
            playerStateRead = playerStateReadRef;
        }

        // Gameplay API
        internal void SetHealth(float value) {
            // state.Health = Mathf.Clamp01(v);
            state.Health = value / 100f;
            state.Notify();
        }

        internal void SetDrunkness(float value) {
            // state.Drunkness = Mathf.Clamp01(v);
            const int maxSteps = 8;
            if (value > maxSteps) { value = maxSteps; }
            state.Drunkness = value / maxSteps;
            state.Notify();
        }

        internal void SetGoal(string text) {
            state.GoalText = text;
            state.Notify();
        }

        internal void SetInteract(bool visible, string text) {
            state.InteractVisible = visible;
            state.InteractText = text;
            state.Notify();
        }

        internal void SetAmmo(int current) {
            state.Ammo = current;
            state.Notify();
        }
    }
}