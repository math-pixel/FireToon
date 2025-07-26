using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game ScriptableObjects")]
public class GameConfig : ScriptableObject
{
    [Header("Player Settings")]
    public int maxPlayers = 4;
    public int minPlayers = 1;
    public Vector3 spawnPosition = Vector3.zero;
    public Vector3 playerScale = Vector3.one * 2.88f;
    public Vector3 playerLocalPosition = new Vector3(0, -0.9f, 0);
}