using UnityEngine;

[CreateAssetMenu(fileName = "VFXConfig", menuName = "Game/VFX ScriptableObjects")]
public class VFXConfig : ScriptableObject
{
    [Header("Firework Settings")]
    public float fireworkLifetime = 5f;
    public bool detachFromParent = true;
    public bool autoDestroy = true;
    
    [Header("Camera Shake")]
    public bool enableCameraShake = true;
    public float shakeIntensity = 1f;
    public float shakeDuration = 0.3f;
    
    [Header("Audio")]
    public AudioClip explosionSound;
    public float explosionVolume = 1f;
    public bool randomizePitch = true;
    public Vector2 pitchRange = new Vector2(0.8f, 1.2f);
    
    [Header("Performance")]
    public int maxSimultaneousEffects = 10;
    public bool poolEffects = true;
}