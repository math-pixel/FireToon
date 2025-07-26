using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Game/Weapon ScriptableObjects")]
public class WeaponConfig : ScriptableObject
{
    [Header("Bullet Settings")]
    public GameObject defaultBulletPrefab;
    public GameObject[] alternativeBullets;
    
    [Header("Shooting Settings")]
    public float fireRate = 1f;
    public bool autoFire = false;
    public int maxAmmo = -1; // -1 = infinite
    
    [Header("Effects")]
    public AudioClip shootSound;
    public GameObject muzzleFlash;
}