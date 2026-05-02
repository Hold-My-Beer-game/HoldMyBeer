using System.Collections;
using HoldMyBeer.Zombies.Contracts;
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

    [Header("Visual Effects")]
    public bool showRaycast = true;
    public Color raycastColor = Color.red;
    public float raycastDuration = 0.1f;
    public GameObject[] muzzleFlashObjects;
    public float muzzleDuration;

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

        int loadAmount = Mathf.Min(magazineSize, inventory.CurrentAmmo);
        currentAmmoInMagazine = loadAmount;
        // inventory.CurrentAmmo -= loadAmount;
        inventory.RemoveAmmo(loadAmount);
    }

    // Update is called once per frame
    void Update()
    {
        // Optional: Draw raycast in editor even when not shooting
        if (showRaycast && Application.isEditor)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * range, Color.green);
        }
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
            if (inventory.CurrentAmmo > 0)
            {
                StartReload();
            }
            else
            {
                Debug.Log("No more ammo");
                // Play dry fire animation by name
                // if (animator != null)
                // {
                //     animator.Play("dry_fire", -1, 0f);
                // }
            }

            return;
        }
        Shoot();
        StartCoroutine(MuzzleFlashRoutine());
    }

    private IEnumerator MuzzleFlashRoutine() {
        EnableMuzzleFlash(true);
        yield return new WaitForSeconds(muzzleDuration);
        EnableMuzzleFlash(false);
    }

    private void EnableMuzzleFlash(bool enable) {
        for (int i = 0; i < muzzleFlashObjects.Length; i++) {
            GameObject flash = muzzleFlashObjects[i];
            flash.SetActive(enable);
        }
    }

    private void Shoot()
    {
        currentAmmoInMagazine--;

        // Call shoot animation by name
        if (animator != null)
        {
            animator.Play("shoot", -1, 0f);
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        // Draw raycast for visualization
        if (showRaycast)
        {
            Debug.DrawRay(ray.origin, ray.direction * range, raycastColor, raycastDuration);
        }

        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            // Draw hit point for visualization
            if (showRaycast)
            {
                Debug.DrawLine(ray.origin, hit.point, Color.green, raycastDuration);
                Debug.Log($"Hit: {hit.collider.gameObject.name} at distance: {hit.distance}");
            }

            if (hit.transform.TryGetComponent(out IEnemy enemy))
            {
                enemy.TakeDamage(damage);    
            }
        }
        else
        {
            // Draw full raycast when nothing is hit
            if (showRaycast)
            {
                Debug.DrawRay(ray.origin, ray.direction * range, Color.yellow, raycastDuration);
                Debug.Log($"Shot fired - nothing hit at range: {range}");
            }
        }

        Debug.Log($"Shot fired. In magazine: {currentAmmoInMagazine}, Reserve: {inventory.CurrentAmmo}");

        if (currentAmmoInMagazine <= 0 && inventory.CurrentAmmo > 0)
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        if (isReloading) return;

        if (inventory.CurrentAmmo <= 0) return;
        isReloading = true;
        isShootingBlocked = true;

        Debug.Log("Reloading...");

        // Call reload animation by name
        if (animator != null)
        {
            animator.Play("reload", -1, 0f);
        }

        StartCoroutine(ReloadRoutine());
    }

    private System.Collections.IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - currentAmmoInMagazine;
        int toLoad = Mathf.Min(needed, inventory.CurrentAmmo);

        currentAmmoInMagazine += toLoad;
        // inventory.CurrentAmmo -= toLoad;
        inventory.RemoveAmmo(toLoad);

        isReloading = false;
        isShootingBlocked = false;

        // Optional: Play reload finish animation
        if (animator != null)
        {
            animator.Play("reload_finish", -1, 0f);
        }

        Debug.Log($"Reload complete. In magazine: {currentAmmoInMagazine}, Reserve: {inventory.CurrentAmmo}");
    }

    // Optional: Method to see raycast in Game view with a line renderer
    private void OnDrawGizmos()
    {
        if (showRaycast && playerCamera != null && Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * range);
        }
    }
}