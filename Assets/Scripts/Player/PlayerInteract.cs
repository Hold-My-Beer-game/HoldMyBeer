using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;          
    public float interactDistance = 3f;  

    private PickupItem currentPickup;

    public event Action<bool, string> OnPlayerInteract;

    private PickupItem lastPickup;

    void Update()
    {
        DetectPickup();
    }

    void DetectPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance)) {
            if (!hit.transform.TryGetComponent(out PickupItem pickup)) {
                if (currentPickup) { currentPickup = null; } 
                return;
            }

            currentPickup = pickup;
        }
        else if (currentPickup) { currentPickup = null; } 

        if (currentPickup != lastPickup) {
            bool canInteract = currentPickup != null;
            string interactMsg = canInteract ? "Pickup E" : string.Empty;
            OnPlayerInteract?.Invoke(canInteract, interactMsg);
        }
        
        lastPickup = currentPickup;
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentPickup != null)
        {
            currentPickup.Pickup();
        }
    }
}