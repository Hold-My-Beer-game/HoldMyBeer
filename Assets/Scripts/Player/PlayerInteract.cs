using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;          
    public float interactDistance = 3f;  

    private PickupItem currentPickup;

    void Update()
    {
        DetectPickup();
    }

    void DetectPickup()
    {
        currentPickup = null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            PickupItem pickup = hit.collider.GetComponent<PickupItem>();

            if (pickup != null)
            {
                currentPickup = pickup;
                // Later: show UI prompt "Press E to pick up"
            }
        }
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

