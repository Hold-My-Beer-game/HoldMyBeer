using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Death Settings")]
    public string deathSceneName;

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
        else
        {
            AudioManager.instance.PlayOneShotEvent(SFXEvents.instance.PlayerDamaged, transform.position);
        }
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChange?.Invoke(currentHealth);

        Debug.Log("Player healed. Health: " + currentHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");

        // Unlock and show the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(deathSceneName);
    }
}