using UnityEngine;

namespace HoldMyBeer.UI {
    /// <summary>
    /// Converts gameplay systems into HUD state mutations.
    /// </summary>
    public class HUDController : MonoBehaviour {
        [SerializeField] private HUDView view;
    
        private readonly HUDState state =  new HUDState();
        // private IPlayerStateRead playerStateRead;
        //
        // public void Install(IPlayerStateRead playerStateReadRef) {
            // playerStateRead = playerStateReadRef;
        // }

        // public void Init() {
        //     view.Bind(state);
        //     playerStateRead.OnHealthChange += SetHealth;  
        //     playerStateRead.OnDrunknessChange += SetDrunkness;  
        //     playerStateRead.OnGoalChange += SetGoal;  
        //     playerStateRead.OnInteract += SetInteract;  
        //     playerStateRead.OnAmmoChange += SetAmmo;  
        // }
        
        private void Awake() {
            view.Bind(state);
        }
    
        // Gameplay API
        internal void SetHealth(float v) {
            state.Health = Mathf.Clamp01(v);
            state.Notify();
        }
    
        internal void SetDrunkness(float v) {
            state.Drunkness = Mathf.Clamp01(v);
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
