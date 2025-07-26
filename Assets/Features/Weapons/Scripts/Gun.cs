using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Configuration")]
    public WeaponConfig weaponConfig;
    
    [Header("Fallback")]
    public GameObject fallbackBulletPrefab;
    
    private GameObject currentBulletPrefab;
    private float lastFireTime;
    private int currentAmmo;
    private AudioSource audioSource;

    void Start()
    {
        SetupWeapon();
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        if (!CanShoot()) return;
        
        GameObject bullet = GetCurrentBullet();
        if (bullet == null) return;
        
        GameObject firework = Instantiate(bullet, transform.position, transform.rotation);
        
        // Set up the projectile
        FireworkBehaviour fireworkBehaviour = firework.GetComponent<FireworkBehaviour>();
        if (fireworkBehaviour != null)
        {
            fireworkBehaviour.SetFireworkSender(transform.parent.gameObject);
        }
        
        // Handle ammo
        if (weaponConfig != null && weaponConfig.maxAmmo > 0)
        {
            currentAmmo--;
        }
        
        // Play effects
        PlayShootEffects();
        
        lastFireTime = Time.time;
    }
    
    private bool CanShoot()
    {
        if (weaponConfig == null) return true;
        
        // Check fire rate
        if (Time.time - lastFireTime < 1f / weaponConfig.fireRate)
            return false;
            
        // Check ammo
        if (weaponConfig.maxAmmo > 0 && currentAmmo <= 0)
            return false;
            
        return true;
    }
    
    private GameObject GetCurrentBullet()
    {
        if (currentBulletPrefab != null)
            return currentBulletPrefab;
            
        return fallbackBulletPrefab;
    }
    
    private void SetupWeapon()
    {
        if (weaponConfig != null)
        {
            currentBulletPrefab = weaponConfig.defaultBulletPrefab;
            currentAmmo = weaponConfig.maxAmmo;
        }
        else
        {
            currentBulletPrefab = fallbackBulletPrefab;
            currentAmmo = -1;
        }
    }
    
    private void PlayShootEffects()
    {
        if (weaponConfig == null) return;
        
        // Play sound
        if (weaponConfig.shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(weaponConfig.shootSound);
        }
        
        // Show muzzle flash
        if (weaponConfig.muzzleFlash != null)
        {
            GameObject flash = Instantiate(weaponConfig.muzzleFlash, transform.position, transform.rotation);
            Destroy(flash, 0.1f);
        }
    }
    
    public void ChangeBulletType(int bulletIndex)
    {
        if (weaponConfig == null || weaponConfig.alternativeBullets == null) return;
        
        if (bulletIndex >= 0 && bulletIndex < weaponConfig.alternativeBullets.Length)
        {
            currentBulletPrefab = weaponConfig.alternativeBullets[bulletIndex];
        }
    }
    
    public void ReloadAmmo()
    {
        if (weaponConfig != null)
        {
            currentAmmo = weaponConfig.maxAmmo;
        }
    }
    
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}
