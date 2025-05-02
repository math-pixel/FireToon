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
        if (transform.position.y < -1000 || transform.position.y > 1000 || transform.position.x < -1000 ||
            transform.position.x > 1000)
        {
            Destroy(gameObject);
        }
    }
}
