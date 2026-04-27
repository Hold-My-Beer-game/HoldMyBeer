using UnityEngine;

/// <summary>
/// Converts gameplay systems into HUD state mutations.
/// </summary>
public class HUDController : MonoBehaviour {
    [SerializeField] private HUDView view;
    
    private HUDState state =  new HUDState();

    private void Awake() {
        view.Bind(state);
    }
    
    // Gameplay API
    public void SetHealth(float v) {
        state.Health = Mathf.Clamp01(v);
        state.Notify();
    }
    
    public void SetDrunkness(float v) {
        state.Drunkness = Mathf.Clamp01(v);
        state.Notify();
    }

    public void SetGoal(string text) {
        state.GoalText = text;
        state.Notify();
    }

    public void SetInteract(bool visible, string text) {
        state.InteractVisible = visible;
        state.InteractText = text;
        state.Notify();
    }
    
    public void SetAmmo(int current) {
        state.Ammo = current;
        state.Notify();
    }
}