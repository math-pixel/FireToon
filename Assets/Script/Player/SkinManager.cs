using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
    
    public List<GameObject> SkinList = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSkin(int skinId)
    {
        if (skinId >= 0 && skinId < SkinList.Count)
        {
            GameObject SkinGO = gameObject.transform.Find("Skin").gameObject;
            if (SkinGO.transform.childCount > 0)
            {
                Destroy(SkinGO.transform.GetChild(0).gameObject);
            }
            Instantiate(SkinList[skinId], SkinGO.transform);
        }
    }
}
