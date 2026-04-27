using System;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// Inspector-driven HUD tester.
/// Values set in Inspector are immediately pushed to HUDController.
/// </summary>

[ExecuteAlways]
public class HUDTester : MonoBehaviour {
    [SerializeField] private HUDController controller;
    
    [Header("Vitals")]
    [Range(0, 1)] public float health =1f;
    [Range(0, 1)] public float drunkness = 1f;
    
    [Header("Goal")]
    [TextArea] public string goalText = "Reach the club";

    [Header("Interact")]
    public bool interactVisible = false;
    public string interactText = "Press E";

    [Header("Ammo")]
    public int ammo = 6;

    private void OnValidate() {
        if (controller == null) return;

        Apply();
    }

    private void OnEnable() {
        if (controller == null) return;

        Apply();
    }

    private void Apply() {
        controller.SetHealth(health);
        controller.SetDrunkness(drunkness);
        controller.SetGoal(goalText);
        controller.SetInteract(interactVisible, interactText);
        controller.SetAmmo(ammo);
    }
}