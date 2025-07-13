using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EffectPoolConfig", menuName = "Game/Effect Pool ScriptableObjects")]
public class EffectPoolConfig : ScriptableObject
{
    [Header("Pool Settings")]
    public int initialPoolSize = 10;
    public int maxPoolSize = 50;
    public bool allowPoolGrowth = true;
    
    [Header("Effect Prefabs")]
    public List<GameObject> effectPrefabs = new List<GameObject>();
    
    [Header("Performance")]
    public bool preloadEffects = true;
    public float cleanupInterval = 30f;
    public int maxInactiveTime = 60;
    
    public GameObject GetRandomEffect()
    {
        if (effectPrefabs.Count == 0) return null;
        return effectPrefabs[Random.Range(0, effectPrefabs.Count)];
    }
}