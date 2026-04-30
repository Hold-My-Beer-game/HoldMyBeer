using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Animator animator;

    [Header("Shooting")] 
    public float damage = 25f;
    public float range = 50f;
    public LayerMask layerMask;

    [Header("Ammo")] 
    public int magazineSize = 2;
    public float reloadTime = 1f;

    private int currentAmmoInMagazine;
    private bool isReloading = false;
    private bool isShootingBlocked = false;
    private PlayerInventory inventory;
    
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = GetComponent<PlayerInventory>();
        
        int loadAmount = Mathf.Min(magazineSize, inventory.currentAmmo);
        currentAmmoInMagazine = loadAmount;
        inventory.currentAmmo -= loadAmount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        if (isReloading || isShootingBlocked) return;

        TryShoot();
    }

    private void TryShoot()
    {
        if (currentAmmoInMagazine <= 0)
        {
            if (inventory.currentAmmo > 0)
            {
                StartReload();
            }
            else
            {
                Debug.Log("No more ammo");
            }

            return;
        }
        Shoot();
    }

    private void Shoot()
    {
        currentAmmoInMagazine--;

        if (animator != null)
        {
            animator.SetTrigger("shoot");
        }
        
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            ZombieHealth zombie = hit.collider.GetComponent<ZombieHealth>();

            if (zombie != null)
            {
                zombie.TakeDamage((int)damage);
            }
        }
        Debug.Log($"Shot fired. In magazine: {currentAmmoInMagazine}, Reserve: {inventory.currentAmmo}");

        if (currentAmmoInMagazine <= 0 && inventory.currentAmmo > 0)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        if (isReloading) return;

        if (inventory.currentAmmo <= 0) return;
        isReloading = true;
        isShootingBlocked = true;
        
        Debug.Log("Reloading...");

        if (animator != null)
        {
            animator.SetTrigger("reload");
        }

        StartCoroutine(ReloadRoutine());
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - currentAmmoInMagazine;
        int toLoad = Mathf.Min(needed, inventory.currentAmmo);

        currentAmmoInMagazine += toLoad;
        inventory.currentAmmo -= toLoad;

        isReloading = false;
        isShootingBlocked = false;
    }
    
}
