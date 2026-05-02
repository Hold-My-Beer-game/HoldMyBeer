using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;

    private PickupItem currentPickup;

    public event Action<bool, string> OnPlayerInteract;

    private void Update()
    {
        DetectPickup();
    }

    private void DetectPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        PickupItem newPickup = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)) {
            hit.transform.TryGetComponent(out newPickup);
        }

        SetCurrentPickup(newPickup);
    }

    private void SetCurrentPickup(PickupItem pickup)
    {
        if (ReferenceEquals(pickup, currentPickup)) {
            return;
        }

        currentPickup = pickup;

        bool canInteract = currentPickup != null;
        string interactMsg = canInteract ? "Pickup E" : string.Empty;

        OnPlayerInteract?.Invoke(canInteract, interactMsg);
    }

    private void ForceClearPickup()
    {
        currentPickup = null;
        OnPlayerInteract?.Invoke(false, string.Empty);
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) {
            return;
        }

        if (currentPickup == null) {
            return;
        }

        PickupItem pickup = currentPickup;

        ForceClearPickup();

        pickup.Pickup();
    }
}