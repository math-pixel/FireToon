using UnityEngine;

[CreateAssetMenu(fileName = "ZoneValidationConfig", menuName = "Game/Zone Validation Config")]
public class ZoneValidationConfig : ScriptableObject
{
    [Header("Zone Settings")]
    public float validationDuration = 3f;
    public bool requireAllPlayers = true;
    public int minimumPlayersRequired = 1;
    
    [Header("Visual Feedback")]
    public Color zoneNormalColor = Color.green;
    public Color zoneValidatingColor = Color.yellow;
    public Color zoneReadyColor = Color.blue;
    public float colorTransitionSpeed = 2f;
    
    [Header("Effects")]
    public bool enableParticles = true;
    public bool enableScreenShake = false;
    public float shakeIntensity = 0.1f;
}