using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinCollection", menuName = "Game/Skin Collection")]
public class SkinCollection : ScriptableObject
{
    [Header("Available Skins")]
    public List<GameObject> skins = new List<GameObject>();
    
    public GameObject GetSkin(int index)
    {
        if (index >= 0 && index < skins.Count)
            return skins[index];
        return null;
    }
    
    public int GetSkinCount()
    {
        return skins.Count;
    }
}