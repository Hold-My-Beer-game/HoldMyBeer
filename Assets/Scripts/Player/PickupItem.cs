using UnityEngine;

public enum ItemType
{
    Ammo,
    Alcohol
}

public class PickupItem : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemType itemType;
    public int amount = 1; 

    public void Pickup()
    {
        
        PlayerInventory inventory = FindAnyObjectByType<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogError("No PlayerInventory found in the scene!");
            return;
        }

        switch (itemType)
        {
            case ItemType.Ammo:
                inventory.AddAmmo(amount);
                break;

            case ItemType.Alcohol:
                inventory.AddAlcohol(amount);
                break;
        }

        
        Destroy(gameObject);
    }
}
