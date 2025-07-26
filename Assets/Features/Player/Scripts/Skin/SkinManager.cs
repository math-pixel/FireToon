using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [Header("Configuration")]
    public SkinCollection skinCollection;
    
    private int currentSkin = 0;

    public void NextSkin()
    {
        if (skinCollection == null) return;
        
        currentSkin++;
        if (currentSkin >= skinCollection.GetSkinCount())
        {
            currentSkin = 0;
        }
        ChangeSkin(currentSkin);
    }

    public void ChangeSkin(int skinId)
    {
        if (skinCollection == null) return;
        
        GameObject skinPrefab = skinCollection.GetSkin(skinId);
        if (skinPrefab == null) return;
        
        currentSkin = skinId;
        GameObject skinParent = transform.Find("Skin")?.gameObject;
        
        if (skinParent == null) return;
        
        // Destroy existing skin
        if (skinParent.transform.childCount > 0)
        {
            Destroy(skinParent.transform.GetChild(0).gameObject);
        }
        
        // Instantiate new skin
        GameObject skin = Instantiate(skinPrefab, skinParent.transform);
        skin.transform.localPosition = Vector3.zero;
        
        // Update animator reference
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            Animator skinAnimator = skin.GetComponent<Animator>();
            if (skinAnimator != null)
                playerMovement.SetAnimator(skinAnimator);
        }
    }
}