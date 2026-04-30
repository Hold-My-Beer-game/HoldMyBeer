using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 6;

    public int currentAmmo = 0;

    [Header("Alcohol Settings")]
    public int currentAlcohol = 0;

    
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        Debug.Log("Picked up ammo. Total reserve ammo: " + currentAmmo);
    }

    
    public void AddAlcohol(int amount)
    {
        currentAlcohol += amount;
        Debug.Log("Picked up alcohol. Total bottles: " + currentAlcohol);
    }

    
    public bool UseAmmo(int amount)
    {
        if (currentAmmo >= amount)
        {
            currentAmmo -= amount;
            return true;
        }

        return false;
    }
}
