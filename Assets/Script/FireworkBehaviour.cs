using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkBehaviour : MonoBehaviour
{
    
    [Header("Firework Settings")]
    public float speed = 1f;
    
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " collided with " + gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<LifePlayer>().ReduceLife(1);
        }
    }
}
