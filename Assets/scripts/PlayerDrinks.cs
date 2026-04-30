using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDrink : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    private PlayerInventory inventory;
    private PlayerHealth health;

    [Header("Drink Settings")]
    public float healPercent = 0.25f; 

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
        if (inventory.currentAlcohol <= 0)
        {
            Debug.Log("No alcohol to drink!");
            return;
        }

        StartCoroutine(DrinkRoutine());
    }

    private System.Collections.IEnumerator DrinkRoutine()
    {
        isDrinking = true;

        if (animator != null)
        {
            animator.SetTrigger("drink");
        }

        
        yield return new WaitForSeconds(1.5f);

        
        float healAmount = health.maxHealth * healPercent;
        health.Heal(healAmount);

        
        inventory.currentAlcohol--;

        Debug.Log("Drank alcohol. Remaining bottles: " + inventory.currentAlcohol);

        isDrinking = false;
    }
}