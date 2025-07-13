using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Game/Player ScriptableObjects")]
public class PlayerConfig : ScriptableObject
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float acceleration = 20f;
    public float drag = 4f;
    public float rotationSpeedLerp = 1f;
    
    [Header("Shoot Settings")]
    public float gunBackForce = 100f;
    
    [Header("Life Settings")]
    public int startingLifes = 3;
}