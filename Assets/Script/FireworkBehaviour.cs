using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireworkBehaviour : MonoBehaviour
{
    
    [Header("Firework Settings")]
    public float speed = 1f;
    public int bounceNum = 0;
    public VFX_Firework explosion;
    
    
    private GameObject fireworkSender;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Go forward   
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Destroy
        if (transform.position.z < -100 || transform.position.z > 100 || transform.position.x < -100 ||
            transform.position.x > 100)
        {
            Destroy(gameObject);
        }
    }

    public void setFireworkSender(GameObject sender)
    {
        fireworkSender = sender;
    }

    private void OnTriggerEnter(Collider other)
    {
        // test if ist not sender
        if (fireworkSender.name != other.gameObject.name)
        {
            
            // if  touch reduce life
            Debug.Log(other.gameObject.name + " collided with " + gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<LifePlayer>().ReduceLife(1);
            }

            if (bounceNum <= 0)
            {
                explosion.play();
            }
        
            Destroy(gameObject);
        }
    }
}
