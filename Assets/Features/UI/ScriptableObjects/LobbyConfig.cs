using UnityEngine;

[CreateAssetMenu(fileName = "LobbyConfig", menuName = "Game/Lobby ScriptableObjects")]
public class LobbyConfig : ScriptableObject
{
    [Header("Timer Settings")]
    public float countdownDuration = 3f;
    public bool autoStartWhenReady = true;
    
    [Header("Debug Settings")]
    public KeyCode forceStartKey = KeyCode.Escape;
    public bool enableForceStart = true;
}