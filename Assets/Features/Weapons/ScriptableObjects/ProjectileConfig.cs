using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileConfig", menuName = "Game/Projectile ScriptableObjects")]
public class ProjectileConfig : ScriptableObject
{
    [Header("Movement Settings")]
    public float speed = 1f;
    public float lifetime = 10f;
    
    [Header("Bounce Settings")]
    public int maxBounces = 3;
    public float bounceSpeedMultiplier = 0.8f;
    
    [Header("Damage Settings")]
    public int damage = 1;
    public LayerMask targetLayers = -1;
    
    [Header("Destruction Settings")]
    public Vector2 worldBounds = new Vector2(100f, 100f);
    public bool destroyOnPlayerHit = true;
    public bool destroyOnMaxBounces = true;
    
    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip explosionSound;
}