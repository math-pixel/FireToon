using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFX_Firework : MonoBehaviour
{

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void play()
    {
        transform.parent = null;
        
        // Ajouter le composant VisualEffect
        VisualEffect vfx = gameObject.GetComponent<VisualEffect>();
        

        // Appliquer le Vector3 exposé
        vfx.SetVector3("Position", transform.position);

        // Déclencher l'effet
        vfx.SendEvent("PlayFirework");

        // Optionnel : détruire l'objet après 5s pour éviter les fuites mémoire
        Destroy(gameObject, 5f);
    }
}
