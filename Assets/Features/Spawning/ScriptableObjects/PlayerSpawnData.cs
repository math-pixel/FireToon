using UnityEngine;

[System.Serializable]
public class PlayerSpawnData
{
    public Vector3 position;
    public Vector3 rotation;
    public int priority;
    public bool isVIPSpawn;
}

[CreateAssetMenu(fileName = "PlayerSpawnData", menuName = "Game/Player Spawn Data")]
public class PlayerSpawnCollection : ScriptableObject
{
    [Header("Spawn Data")]
    public PlayerSpawnData[] spawnData;
    
    [Header("Settings")]
    public bool randomizeSpawns = false;
    public bool prioritizeVIPSpawns = true;
    
    public PlayerSpawnData GetSpawnData(int index)
    {
        if (index >= 0 && index < spawnData.Length)
            return spawnData[index];
        return null;
    }
    
    public int GetSpawnCount()
    {
        return spawnData?.Length ?? 0;
    }
}