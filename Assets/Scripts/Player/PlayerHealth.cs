using FMOD.Studio;
using System;
using UnityEngine;
using HoldMyBeer.Audio;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    public event Action<float> OnHealthChange;
    public event Action OnDeath;

    private EventInstance heartbeat;

    private EventInstance playerDmg;

    private MusicSelection music;

    void Start()
    {
        music = FindFirstObjectByType<MusicSelection>();
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(currentHealth);
        heartbeat = AudioManager.instance.CreateEventInstance(SFXEvents.instance.LowHpHeartbeat);
        playerDmg = AudioManager.instance.CreateEventInstance(SFXEvents.instance.PlayerDamaged);
        heartbeat.start();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChange?.Invoke(currentHealth);

        Debug.Log("Player took damage. Health: " + currentHealth);

        if (currentHealth <= 0f)
        {
            AudioManager.instance.PlayOneShotEvent(SFXEvents.instance.PlayerDeath, transform.position);
            AudioManager.instance.CleanUp();
            music.StopMusic();
            Die();
        }
        else
        {
            playerDmg.start();
        }
    }

    public void Update() {
        AudioManager.instance.SetParameter(heartbeat, "PlayerHealth", currentHealth);
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
        OnDeath?.Invoke();
    }
}