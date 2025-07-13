using UnityEngine;
using System.Collections;

public class FireworkBehaviour : MonoBehaviour
{
    [Header("Configuration")]
    public ProjectileConfig projectileConfig;
    
    [Header("Fallback Settings")]
    public float fallbackSpeed = 10f; // ⭐ Augmenté la valeur par défaut
    public int fallbackMaxBounces = 3;
    public VFX_Firework explosion;
    
    [Header("Debug")]
    public bool enableDebugLogs = false;
    
    private GameObject fireworkSender;
    private int currentBounce = 0;
    private Rigidbody rb;
    private Vector3 lastVelocity;
    private float startTime;
    private AudioSource audioSource;
    private bool isDestroyed = false; // ⭐ Protection contre double destruction
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        startTime = Time.time;
        
        if (enableDebugLogs) Debug.Log($"Firework created: {gameObject.name}");
        
        // ⭐ CORRECTION : Donner une vélocité initiale immédiatement
        InitializeMovement();
        
        // Start lifetime countdown
        if (projectileConfig != null && projectileConfig.lifetime > 0)
        {
            StartCoroutine(LifetimeCountdown());
        }
        else
        {
            // Fallback lifetime si pas de config
            StartCoroutine(LifetimeCountdown(10f));
        }
    }
    
    private void InitializeMovement()
    {
        float speed = projectileConfig != null ? projectileConfig.speed : fallbackSpeed;
        rb.linearVelocity = transform.forward * speed; // ⭐ Vélocité initiale
        lastVelocity = rb.linearVelocity;
        
        if (enableDebugLogs) Debug.Log($"Initial velocity set: {rb.linearVelocity}");
    }
    
    private IEnumerator LifetimeCountdown(float? customLifetime = null)
    {
        float lifetime = customLifetime ?? projectileConfig.lifetime;
        yield return new WaitForSeconds(lifetime);
        
        if (!isDestroyed)
        {
            if (enableDebugLogs) Debug.Log("Firework lifetime expired");
            DestroyProjectile();
        }
    }

    void FixedUpdate() // ⭐ Changé de LateUpdate à FixedUpdate
    {
        if (isDestroyed) return;
        
        UpdateMovement();
        CheckWorldBounds();
    }
    
    private void UpdateMovement()
    {
        // ⭐ CORRECTION : Ne pas modifier la vélocité en continu
        // La vélocité est définie une fois au Start et modifiée seulement lors des rebonds
        lastVelocity = rb.linearVelocity;
        
        // Optionnel : Appliquer une légère gravité ou résistance de l'air
        if (projectileConfig != null && projectileConfig.speed > 0)
        {
            // Maintenir la vitesse constante (ignorer la gravité)
            Vector3 currentDirection = rb.linearVelocity.normalized;
            rb.linearVelocity = currentDirection * projectileConfig.speed;
        }
    }
    
    private void CheckWorldBounds()
    {
        Vector2 bounds = projectileConfig != null ? projectileConfig.worldBounds : new Vector2(100f, 100f);
        
        if (Mathf.Abs(transform.position.z) > bounds.y || Mathf.Abs(transform.position.x) > bounds.x)
        {
            if (enableDebugLogs) Debug.Log("Firework out of bounds");
            DestroyProjectile();
        }
    }

    public void SetFireworkSender(GameObject sender)
    {
        fireworkSender = sender;
        if (enableDebugLogs) Debug.Log($"Firework sender set: {sender?.name}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDestroyed) return; // ⭐ Protection
        
        GameObject hitObject = collision.gameObject;
        
        if (enableDebugLogs) Debug.Log($"Firework collision with: {hitObject.name}");
        
        // Ignore collision with sender and other projectiles
        if (IsIgnoredCollision(hitObject)) return;
        
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
        // ⭐ CORRECTION : Vérifications plus strictes
        if (fireworkSender != null && 
            (fireworkSender == hitObject || fireworkSender.name == hitObject.name))
        {
            if (enableDebugLogs) Debug.Log("Ignored collision with sender");
            return true;
        }
            
        if (hitObject == gameObject)
        {
            if (enableDebugLogs) Debug.Log("Ignored self collision");
            return true;
        }
        
        // ⭐ Ignorer les autres projectiles
        if (hitObject.GetComponent<FireworkBehaviour>() != null)
        {
            if (enableDebugLogs) Debug.Log("Ignored collision with other projectile");
            return true;
        }
            
        // Check if target layer is valid
        if (projectileConfig != null)
        {
            int hitLayer = 1 << hitObject.layer;
            if ((projectileConfig.targetLayers.value & hitLayer) == 0)
            {
                if (enableDebugLogs) Debug.Log("Ignored collision - wrong layer");
                return true;
            }
        }
        
        return false;
    }
    
    private void HandlePlayerHit(GameObject player)
    {
        if (enableDebugLogs) Debug.Log($"Player hit: {player.name}");
        
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
                if (enableDebugLogs) Debug.Log("Max bounces reached");
                DestroyProjectile();
                return;
            }
        }
        
        // Calculate bounce direction
        Vector3 bounceDirection = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
        
        // Apply speed multiplier
        float currentSpeed = lastVelocity.magnitude;
        float speedMultiplier = projectileConfig != null ? projectileConfig.bounceSpeedMultiplier : 0.8f;
        currentSpeed *= speedMultiplier;
        
        // ⭐ CORRECTION : Appliquer la nouvelle vélocité correctement
        rb.linearVelocity = bounceDirection * currentSpeed;
        transform.rotation = Quaternion.LookRotation(bounceDirection);
        
        currentBounce++;
        
        if (enableDebugLogs) Debug.Log($"Bounce {currentBounce}/{maxBounces}, new velocity: {rb.linearVelocity}");
    }
    
    private void DestroyProjectile()
    {
        if (isDestroyed) return; // ⭐ Protection contre double destruction
        
        isDestroyed = true;
        
        if (enableDebugLogs) Debug.Log("Destroying projectile");
        
        PlayExplosionEffects();
        
        // ⭐ Destruction immédiate pour éviter les problèmes
        Destroy(gameObject);
    }
    
    private void PlayExplosionEffects()
    {
        // ⭐ CORRECTION : Créer les effets AVANT la destruction
        Vector3 explosionPosition = transform.position;
        Quaternion explosionRotation = transform.rotation;
        
        // Play VFX explosion
        if (explosion != null)
        {
            // ⭐ Détacher l'explosion du projectile
            explosion.transform.parent = null;
            explosion.play();
        }
        
        // Play explosion effect from config
        if (projectileConfig != null && projectileConfig.explosionEffect != null)
        {
            GameObject effect = Instantiate(projectileConfig.explosionEffect, explosionPosition, explosionRotation);
            Destroy(effect, 5f);
        }
        
        // Play explosion sound
        if (projectileConfig != null && projectileConfig.explosionSound != null)
        {
            // ⭐ Jouer le son via AudioSource.PlayClipAtPoint pour éviter la destruction
            AudioSource.PlayClipAtPoint(projectileConfig.explosionSound, explosionPosition);
        }
    }
    
    // ⭐ Méthodes de debug
    [ContextMenu("Debug Info")]
    public void DebugInfo()
    {
        Debug.Log($"=== FIREWORK DEBUG ===");
        Debug.Log($"Position: {transform.position}");
        Debug.Log($"Velocity: {rb.linearVelocity}");
        Debug.Log($"Speed: {rb.linearVelocity.magnitude}");
        Debug.Log($"Bounces: {currentBounce}");
        Debug.Log($"Time alive: {Time.time - startTime}");
        Debug.Log($"Sender: {fireworkSender?.name ?? "None"}");
    }
    
    void OnDestroy()
    {
        if (enableDebugLogs) Debug.Log($"Firework destroyed: {gameObject.name}");
    }
}
