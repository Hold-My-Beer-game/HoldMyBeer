using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmo = 6;

    [SerializeField] private int currentAmmo = 0;

    [Header("Alcohol Settings")]
    [SerializeField] private int currentAlcohol = 0;
    
    public int CurrentAmmo => currentAmmo;
    public int CurrentAlcohol => currentAlcohol;

    public event Action<float> OnAlcoholChange;
    public event Action<float> OnAmmoChange;

    private void Start() {
        OnAmmoChange?.Invoke(currentAmmo);
        OnAlcoholChange?.Invoke(currentAlcohol);
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        OnAmmoChange?.Invoke(currentAmmo);
        Debug.Log("Picked up ammo. Total reserve ammo: " + currentAmmo);
    }
    public bool HasMaxAmmo()
    {
        return currentAmmo >= maxAmmo;
    }

    public void AddAlcohol(int amount)
    {
        currentAlcohol += amount;
        OnAlcoholChange?.Invoke(currentAlcohol);
        Debug.Log("Picked up alcohol. Total bottles: " + currentAlcohol);
    }

    public void RemoveAmmo(int amount) {
        currentAmmo = Mathf.Clamp(currentAmmo - amount, 0, maxAmmo);
        OnAmmoChange?.Invoke(currentAmmo);
    }

    public void RemoveAlcohol(int amount) {
        currentAlcohol -= amount;
        currentAlcohol = Mathf.Max(0, currentAlcohol);
        OnAlcoholChange?.Invoke(currentAlcohol);
    }
}
