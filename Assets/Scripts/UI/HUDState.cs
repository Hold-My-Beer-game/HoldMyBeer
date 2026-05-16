using System;

namespace HoldMyBeer.UI {
    /// <summary>
    /// Pure gameplay state.
    /// No UI Toolkit dependency.
    /// No binding system dependency.
    /// </summary>
    internal class HUDState {
        public float Drunkness;
        public float Health;

        // todo goal visibility?
        public string GoalText ="Reach the Club";
        public bool InteractVisible;
        public string InteractText;

        public int Ammo;

        // Event-driven update signal (single channel)
        public event Action OnChanged;

        public void Notify() => OnChanged?.Invoke();
    }
}