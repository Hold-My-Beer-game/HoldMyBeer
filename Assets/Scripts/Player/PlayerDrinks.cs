using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDrink : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    private PlayerInventory inventory;
    private PlayerHealth health;

    [Header("Drink Settings")]
    public float healPercent = 0.25f;
    public float drinkDuration = 1.5f;

    private bool isDrinking = false;

    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        health = GetComponent<PlayerHealth>();
    }

    public void OnDrink(InputValue value)
    {
        if (!value.isPressed) return;
        if (isDrinking) return;

        TryDrink();
    }

    private void TryDrink()
    {
        if (inventory.CurrentAlcohol <= 0)
        {
            Debug.Log("No alcohol to drink!");

            // Optional: Play dry drink animation or error animation
            if (animator != null)
            {
                animator.Play("drink_empty", -1, 0f);
            }
            return;
        }

        StartCoroutine(DrinkRoutine());
    }

    private IEnumerator DrinkRoutine()
    {
        isDrinking = true;

        // Call drink animation by name
        if (animator != null)
        {
            animator.Play("drink", -1, 0f);
        }

        // Wait for drink animation to play
        yield return new WaitForSeconds(drinkDuration);

        // Calculate and apply healing
        float healAmount = health.maxHealth * healPercent;
        health.Heal(healAmount);

        // Consume one alcohol bottle
        inventory.RemoveAlcohol(inventory.CurrentAlcohol-1);

        Debug.Log($"Drank alcohol. Healed {healAmount} health. Remaining bottles: {inventory.CurrentAlcohol}");

        // Optional: Play drink finish animation
        if (animator != null)
        {
            animator.Play("drink_finish", -1, 0f);
        }

        isDrinking = false;
    }

    // Optional: Method to cancel drinking mid-way
    public void CancelDrink()
    {
        if (isDrinking)
        {
            StopAllCoroutines();
            isDrinking = false;
            Debug.Log("Drinking cancelled");
        }
    }
}