using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    
    public List<GameObject> SkinList = new List<GameObject>();
    
    private int currentSkin = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentSkin++;
            if (currentSkin > SkinList.Count - 1)
            {
                currentSkin = 0;
            }
            ChangeSkin(currentSkin);
        }
        
    }

    public void ChangeSkin(int skinId)
    {
        if (skinId >= 0 && skinId < SkinList.Count)
        {
            currentSkin = skinId;
            GameObject SkinParent = gameObject.transform.Find("Skin").gameObject;
            if (SkinParent.transform.childCount > 0)
            {
                Destroy(SkinParent.transform.GetChild(0).gameObject);
            }
            GameObject skin = Instantiate(SkinList[skinId], SkinParent.transform);
            skin.transform.localPosition = Vector3.zero;
            gameObject.GetComponent<PlayerMovement>().animator = skin.gameObject.GetComponent<Animator>();
        }
    }
}
