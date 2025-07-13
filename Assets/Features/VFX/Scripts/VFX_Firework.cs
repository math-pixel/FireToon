using UnityEngine;
using Cinemachine;
using System.Collections;

public class VFX_Firework : MonoBehaviour
{
    [Header("Configuration")]
    public VFXConfig vfxConfig;
    
    [Header("References")]
    public CinemachineImpulseSource impulseSource;
    
    private AudioSource audioSource;
    private static int activeEffectsCount = 0;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Play()
    {
        // Check max simultaneous effects
        if (vfxConfig != null && activeEffectsCount >= vfxConfig.maxSimultaneousEffects)
        {
            Destroy(gameObject);
            return;
        }
        
        activeEffectsCount++;
        
        // Detach from parent if configured
        if (vfxConfig == null || vfxConfig.detachFromParent)
        {
            transform.parent = null;
        }

        // Play particle systems
        PlayParticles();
        
        // Play audio
        PlayAudio();
        
        // Shake camera
        ShakeCamera();

        // Auto destroy if configured
        if (vfxConfig == null || vfxConfig.autoDestroy)
        {
            float lifetime = vfxConfig?.fireworkLifetime ?? 5f;
            StartCoroutine(DestroyAfterDelay(lifetime));
        }
    }
    
    private void PlayParticles()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }
    
    private void PlayAudio()
    {
        if (vfxConfig?.explosionSound != null && audioSource != null)
        {
            audioSource.clip = vfxConfig.explosionSound;
            audioSource.volume = vfxConfig.explosionVolume;
            
            if (vfxConfig.randomizePitch)
            {
                audioSource.pitch = Random.Range(vfxConfig.pitchRange.x, vfxConfig.pitchRange.y);
            }
            
            audioSource.Play();
        }
    }
    
        private void ShakeCamera()
    {
        if (vfxConfig == null || !vfxConfig.enableCameraShake) return;
        
        if (impulseSource != null)
        {
            // Configure impulse source with config values
            impulseSource.m_ImpulseDefinition.m_ImpulseDuration = vfxConfig.shakeDuration;
            impulseSource.m_DefaultVelocity = Vector3.one * vfxConfig.shakeIntensity;
            impulseSource.GenerateImpulse();
        }
    }
    
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        activeEffectsCount--;
        
        // Ensure count doesn't go negative
        if (activeEffectsCount < 0)
            activeEffectsCount = 0;
            
        Destroy(gameObject);
    }
    
    public void Stop()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        
        foreach (ParticleSystem particle in particles)
        {
            particle.Stop();
        }
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    public void SetCustomLifetime(float lifetime)
    {
        if (vfxConfig != null)
        {
            // Create a runtime copy to avoid modifying the original ScriptableObject
            vfxConfig = Instantiate(vfxConfig);
            vfxConfig.fireworkLifetime = lifetime;
        }
    }
    
    public void SetCustomIntensity(float intensity)
    {
        if (vfxConfig != null)
        {
            if (!vfxConfig.name.Contains("(Clone)"))
                vfxConfig = Instantiate(vfxConfig);
            vfxConfig.shakeIntensity = intensity;
        }
    }
    
    public static int GetActiveEffectsCount()
    {
        return activeEffectsCount;
    }
    
    public static void ResetEffectsCount()
    {
        activeEffectsCount = 0;
    }
    
    void OnDestroy()
    {
        activeEffectsCount--;
        if (activeEffectsCount < 0)
            activeEffectsCount = 0;
    }
    
    // Method to be called from old code for compatibility
    public void play()
    {
        Play();
    }
}
