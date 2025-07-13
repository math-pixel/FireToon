using UnityEngine;
using System.Collections;

public class FireworkBehaviour : MonoBehaviour
{
    [Header("Configuration")]
    public ProjectileConfig projectileConfig;
    
    [Header("Fallback Settings")]
    public float fallbackSpeed = 1f;
    public int fallbackMaxBounces = 3;
    public VFX_Firework explosion;
    
    private GameObject fireworkSender;
    private int currentBounce = 0;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private Vector3 directionBullet;
    private float startTime;
    private AudioSource audioSource;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        directionBullet = transform.forward;
        startTime = Time.time;
        
        // Start lifetime countdown
        if (projectileConfig != null && projectileConfig.lifetime > 0)
        {
            StartCoroutine(LifetimeCountdown());
        }
    }
    
    private IEnumerator LifetimeCountdown()
    {
        yield return new WaitForSeconds(projectileConfig.lifetime);
        DestroyProjectile();
    }

    void LateUpdate()
    {
        MoveProjectile();
        CheckWorldBounds();
    }
    
    private void MoveProjectile()
    {
        float speed = projectileConfig != null ? projectileConfig.speed : fallbackSpeed;
        rb.linearVelocity = transform.forward * speed * Time.deltaTime;
        lastVelocity = rb.linearVelocity;
    }
    
    private void CheckWorldBounds()
    {
        Vector2 bounds = projectileConfig != null ? projectileConfig.worldBounds : new Vector2(100f, 100f);
        
        if (Mathf.Abs(transform.position.z) > bounds.y || Mathf.Abs(transform.position.x) > bounds.x)
        {
            DestroyProjectile();
        }
    }

    public void SetFireworkSender(GameObject sender)
    {
        fireworkSender = sender;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        
        // Ignore collision with sender and other projectiles
        if (IsIgnoredCollision(hitObject)) return;
        
        Debug.Log($"Firework hit {hitObject.name}");
        
        // Handle player hit
        if (hitObject.CompareTag("Player"))
        {
            HandlePlayerHit(hitObject);
            return;
        }
        
        // Handle bounce
        HandleBounce(collision);
    }
    
    private bool IsIgnoredCollision(GameObject hitObject)
    {
        if (fireworkSender != null && fireworkSender.name == hitObject.name)
            return true;
            
        if (hitObject.name == gameObject.name)
            return true;
            
        // Check if target layer is valid
        if (projectileConfig != null)
        {
            int hitLayer = 1 << hitObject.layer;
            if ((projectileConfig.targetLayers.value & hitLayer) == 0)
                return true;
        }
        
        return false;
    }
    
    private void HandlePlayerHit(GameObject player)
    {
        LifePlayer lifePlayer = player.GetComponent<LifePlayer>();
        if (lifePlayer != null)
        {
            int damage = projectileConfig != null ? projectileConfig.damage : 1;
            lifePlayer.ReduceLife(damage);
        }
        
        bool destroyOnHit = projectileConfig != null ? projectileConfig.destroyOnPlayerHit : true;
        if (destroyOnHit)
        {
            DestroyProjectile();
        }
    }
    
    private void HandleBounce(Collision collision)
    {
        int maxBounces = projectileConfig != null ? projectileConfig.maxBounces : fallbackMaxBounces;
        
        if (currentBounce >= maxBounces)
        {
            bool destroyOnMaxBounces = projectileConfig != null ? projectileConfig.destroyOnMaxBounces : true;
            if (destroyOnMaxBounces)
            {
                DestroyProjectile();
                return;
            }
        }
        
        // Calculate bounce direction
        float currentSpeed = lastVelocity.magnitude;
        Vector3 bounceDirection = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
        
        // Apply speed multiplier
        float speedMultiplier = projectileConfig != null ? projectileConfig.bounceSpeedMultiplier : 0.8f;
        currentSpeed *= speedMultiplier;
        
        directionBullet = bounceDirection * currentSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(bounceDirection);
        
        currentBounce++;
    }
    
    private void DestroyProjectile()
    {
        PlayExplosionEffects();
        Destroy(gameObject);
    }
    
    private void PlayExplosionEffects()
    {
        // Play VFX explosion
        if (explosion != null)
        {
            explosion.play();
        }
        
        // Play explosion effect from config
        if (projectileConfig != null && projectileConfig.explosionEffect != null)
        {
            GameObject effect = Instantiate(projectileConfig.explosionEffect, transform.position, transform.rotation);
            Destroy(effect, 5f); // Clean up after 5 seconds
        }
        
        // Play explosion sound
        if (projectileConfig != null && projectileConfig.explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(projectileConfig.explosionSound);
        }
    }
    
    public void SetCustomSpeed(float speed)
    {
        if (projectileConfig != null)
        {
            // Create a runtime copy to avoid modifying the original ScriptableObject
            projectileConfig = Instantiate(projectileConfig);
            projectileConfig.speed = speed;
        }
    }
    
    public void SetCustomDamage(int damage)
    {
        if (projectileConfig != null)
        {
            if (projectileConfig.name.Contains("(Clone)") == false)
                projectileConfig = Instantiate(projectileConfig);
            projectileConfig.damage = damage;
        }
    }
    
    public int GetCurrentBounces()
    {
        return currentBounce;
    }
    
    public float GetTimeAlive()
    {
        return Time.time - startTime;
    }
}
