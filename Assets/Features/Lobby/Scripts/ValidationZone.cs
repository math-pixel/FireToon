using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ValidationZone : MonoBehaviour
{
    [Header("Configuration")]
    public ZoneValidationConfig zoneConfig;
    
    [Header("References")]
    public Renderer zoneRenderer;
    public ParticleSystem validationParticles;
    public AudioSource audioSource;
    
    [Header("Events")]
    public UnityEvent onValidationStart;
    public UnityEvent onValidationComplete;
    public UnityEvent onValidationCancelled;
    public UnityEvent<int, int> onPlayerCountChanged; // current, required
    
    private HashSet<GameObject> playersInZone = new HashSet<GameObject>();
    private bool isValidating = false;
    private Coroutine validationCoroutine;
    private Material zoneMaterial;
    private Color originalColor;
    
    public int PlayersInZone => playersInZone.Count;
    public int RequiredPlayers => GetRequiredPlayerCount();
    public bool IsValidating => isValidating;
    
    void Start()
    {
        InitializeZone();
    }
    
    private void InitializeZone()
    {
        // Setup material
        if (zoneRenderer != null)
        {
            zoneMaterial = zoneRenderer.material;
            originalColor = zoneConfig != null ? zoneConfig.zoneNormalColor : Color.green;
            zoneMaterial.color = originalColor;
        }
        
        // Setup audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // Setup particles
        if (validationParticles != null)
            validationParticles.Stop();
            
        Debug.Log($"ValidationZone initialized. Required players: {RequiredPlayers}");
    }
    
    private int GetRequiredPlayerCount()
    {
        if (zoneConfig == null) return 1;
        
        if (zoneConfig.requireAllPlayers && PlayerRegistry.Instance != null)
        {
            return PlayerRegistry.Instance.RegisteredPlayers.Count;
        }
        
        return zoneConfig.minimumPlayersRequired;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            AddPlayer(other.gameObject);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other.gameObject))
        {
            RemovePlayer(other.gameObject);
        }
    }
    
    private bool IsPlayer(GameObject obj)
    {
        return obj.CompareTag("Player") || obj.GetComponent<UnityEngine.InputSystem.PlayerInput>() != null;
    }
    
    private void AddPlayer(GameObject player)
    {
        if (playersInZone.Add(player))
        {
            Debug.Log($"Player {player.name} entered validation zone. ({PlayersInZone}/{RequiredPlayers})");
            
            // Play enter sound
            PlaySound(zoneConfig?.playerEnterSound);
            
            // Update UI/Events
            onPlayerCountChanged?.Invoke(PlayersInZone, RequiredPlayers);
            
            // Check if we can start validation
            CheckValidationConditions();
        }
    }
    
    private void RemovePlayer(GameObject player)
    {
        if (playersInZone.Remove(player))
        {
            Debug.Log($"Player {player.name} left validation zone. ({PlayersInZone}/{RequiredPlayers})");
            
            // Play exit sound
            PlaySound(zoneConfig?.playerExitSound);
            
            // Update UI/Events
            onPlayerCountChanged?.Invoke(PlayersInZone, RequiredPlayers);
            
            // Cancel validation if not enough players
            if (isValidating && !HasEnoughPlayers())
            {
                CancelValidation();
            }
        }
    }
    
    private bool HasEnoughPlayers()
    {
        return PlayersInZone >= RequiredPlayers;
    }
    
    private void CheckValidationConditions()
    {
        if (!isValidating && HasEnoughPlayers())
        {
            StartValidation();
        }
    }
    
    private void StartValidation()
    {
        if (isValidating) return;
        
        isValidating = true;
        Debug.Log($"Starting validation countdown ({zoneConfig?.validationDuration ?? 3f} seconds)");
        
        // Visual feedback
        ChangeZoneColor(zoneConfig?.zoneValidatingColor ?? Color.yellow);
        
        // Audio feedback
        PlaySound(zoneConfig?.validationStartSound);
        
        // Particle effects
        if (validationParticles != null && zoneConfig?.enableParticles == true)
        {
            validationParticles.Play();
        }
        
        // Events
        onValidationStart?.Invoke();
        
        // Start countdown
        float duration = zoneConfig?.validationDuration ?? 3f;
        validationCoroutine = StartCoroutine(ValidationCountdown(duration));
    }
    
    private void CancelValidation()
    {
        if (!isValidating) return;
        
        isValidating = false;
        Debug.Log("Validation cancelled - not enough players");
        
        // Stop countdown
        if (validationCoroutine != null)
        {
            StopCoroutine(validationCoroutine);
            validationCoroutine = null;
        }
        
        // Visual feedback
        ChangeZoneColor(originalColor);
        
        // Stop particles
        if (validationParticles != null)
        {
            validationParticles.Stop();
        }
        
        // Events
        onValidationCancelled?.Invoke();
    }
    
    private IEnumerator ValidationCountdown(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            // Check if we still have enough players
            if (!HasEnoughPlayers())
            {
                CancelValidation();
                yield break;
            }
            
            elapsed += Time.deltaTime;
            
            // Optional: Update visual progress
            float progress = elapsed / duration;
            UpdateValidationProgress(progress);
            
            yield return null;
        }
        
        // Validation complete!
        CompleteValidation();
    }
    
    private void UpdateValidationProgress(float progress)
    {
        // Optional: Change color intensity based on progress
        if (zoneConfig != null && zoneMaterial != null)
        {
            Color currentColor = Color.Lerp(zoneConfig.zoneValidatingColor, zoneConfig.zoneReadyColor, progress);
            zoneMaterial.color = currentColor;
        }
        
        // Optional: Scale particles based on progress
        if (validationParticles != null)
        {
            var emission = validationParticles.emission;
            emission.rateOverTime = Mathf.Lerp(10f, 50f, progress);
        }
    }
    
    private void CompleteValidation()
    {
        isValidating = false;
        Debug.Log("Validation complete! Starting game...");
        
        // Visual feedback
        ChangeZoneColor(zoneConfig?.zoneReadyColor ?? Color.blue);
        
        // Audio feedback
        PlaySound(zoneConfig?.validationCompleteSound);
        
        // Screen shake
        if (zoneConfig?.enableScreenShake == true)
        {
            // Implement screen shake here if you have a camera shake system
            Camera.main?.transform.DOShakePosition(0.5f, zoneConfig.shakeIntensity);
        }
        
        // Events
        onValidationComplete?.Invoke();
        
        // Start game transition
        StartGameTransition();
    }
    
    private void StartGameTransition()
    {
        // Notify LobbyManager or GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RequestStateChange(GameManager.GameState.Playing);
        }
    }
    
    private void ChangeZoneColor(Color targetColor)
    {
        if (zoneMaterial != null && zoneConfig != null)
        {
            zoneMaterial.DOColor(targetColor, 1f / zoneConfig.colorTransitionSpeed);
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    // Public methods for external control
    public void ForceStartValidation()
    {
        if (HasEnoughPlayers())
        {
            StartValidation();
        }
    }
    
    public void ForceCompleteValidation()
    {
        if (isValidating)
        {
            StopCoroutine(validationCoroutine);
            CompleteValidation();
        }
    }
    
    public void ResetZone()
    {
        CancelValidation();
        playersInZone.Clear();
        onPlayerCountChanged?.Invoke(0, RequiredPlayers);
    }
    
    // Debug methods
    [ContextMenu("Debug - List Players In Zone")]
    public void DebugListPlayers()
    {
        Debug.Log($"Players in zone ({PlayersInZone}):");
        foreach (var player in playersInZone)
        {
            Debug.Log($"- {player.name}");
        }
    }
    
    void OnDestroy()
    {
        if (validationCoroutine != null)
        {
            StopCoroutine(validationCoroutine);
        }
    }
}
