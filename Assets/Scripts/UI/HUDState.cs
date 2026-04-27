using System;

/// <summary>
/// Pure gameplay state.
/// No UI Toolkit dependency.
/// No binding system dependency.
/// </summary>
public class HUDState {
    public float Drunkness;
    public float Health;

    public string GoalText;
    public bool InteractVisible;
    public string InteractText;

    public int Ammo;
    
    // Event-driven update signal (single channel)
    public event Action OnChanged;
    
    public void Notify() => OnChanged?.Invoke();
}