using UnityEngine;

[CreateAssetMenu(fileName = "SpawnConfig", menuName = "Game/Spawn ScriptableObjects")]
public class SpawnConfig : ScriptableObject
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    
    [Header("Animation Settings")]
    public string winnerAnimationName = "EndingAnimationWinner";
    public string loserAnimationName = "EndingAnimationLooser";
    
    [Header("Scoreboard Settings")]
    public bool disableMovementOnScoreboard = true;
    public bool removeGunsOnScoreboard = true;
}