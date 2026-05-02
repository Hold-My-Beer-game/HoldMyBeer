using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    public event Action<float> OnHealthChange;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(currentHealth);
    }
    

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChange?.Invoke(currentHealth);
        
        Debug.Log("Player took damage. Health: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float heal) {
        currentHealth += heal;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChange?.Invoke(currentHealth);
        
        Debug.Log("Player healed. Health: " + currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");
    }
}
