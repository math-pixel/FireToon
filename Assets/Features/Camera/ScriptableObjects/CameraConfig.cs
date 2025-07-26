using UnityEngine;

[CreateAssetMenu(fileName = "CameraConfig", menuName = "Game/Camera ScriptableObjects")]
public class CameraConfig : ScriptableObject
{
    [Header("Target Group Settings")]
    public float defaultWeight = 1f;
    public float defaultRadius = 5f;
    
    [Header("Camera Behavior")]
    public bool autoAddPlayers = true;
    public bool removeDeadPlayers = false;
    public float transitionSpeed = 2f;
}