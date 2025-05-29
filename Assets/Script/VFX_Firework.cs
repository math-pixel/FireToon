using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class VFX_Firework : MonoBehaviour
{

    public CinemachineImpulseSource impulseSource;
    
    
    
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

        Component[] particles = gameObject.GetComponentsInChildren<ParticleSystem>();

        // display particles
        foreach (ParticleSystem particle in particles)
        {
            particle.Play();
        }
        
        // Shake the cam
        impulseSource.GenerateImpulse();

        // Optionnel : détruire l'objet après 5s pour éviter les fuites mémoire
        Destroy(gameObject, 5f);
    }
}
