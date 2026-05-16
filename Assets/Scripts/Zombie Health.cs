using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public float delayBeforeDeactivating = 1f;

    [Header("Effects")]
    public GameObject deathEffect;
    public AudioClip deathSound;
    public AudioClip hitSound;

    [Header("Components")]
    private Animator animator;
    private AudioSource audioSource;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();


        // Add AudioSource if it doesn't exist
        if (audioSource == null && (deathSound != null || hitSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Play hit effect
        PlayHitEffect();

        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Optional: Trigger hurt animation
            if (animator != null)
            {
                animator.SetTrigger("hurt");
            }
        }
    }

    private void PlayHitEffect()
    {
        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Optional: Flash red effect
        StartCoroutine(FlashRed());
    }

    private System.Collections.IEnumerator FlashRed()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Color originalColor = Color.white;

        // Store original colors
        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = renderers[i].material.color;
                renderers[i].material.color = Color.red;
            }
        }

        yield return new WaitForSeconds(0.1f);

        // Restore original colors
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log($"{gameObject.name} died! Deactivating game object.");

        // Play death sound
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Play death animation (optional - you might want to let animation play before deactivating)
        if (animator != null)
        {
            animator.SetTrigger("death");
        }

        // Start coroutine to deactivate after a delay (to allow death sound/effect to play)
        StartCoroutine(DeactivateAfterDelay());
    }

    private System.Collections.IEnumerator DeactivateAfterDelay()
    {
        // Wait for death sound to finish or a short delay
        if (deathSound != null)
        {
            delayBeforeDeactivating = deathSound.length;
        }

        yield return new WaitForSeconds(delayBeforeDeactivating);

        // Deactivate the game object instead of destroying it
        gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        if (!isDead)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            Debug.Log($"{gameObject.name} healed for {amount}. Health: {currentHealth}/{maxHealth}");
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    // Optional: Method to respawn/reactivate the zombie
    public void Respawn()
    {
        if (isDead)
        {
            currentHealth = maxHealth;
            isDead = false;

            // Re-enable collider if it was disabled
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            // Reset any other components as needed
            Debug.Log($"{gameObject.name} respawned with full health!");
        }
    }
}