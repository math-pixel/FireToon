using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    
    public GameObject bulletPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        GameObject firework = Instantiate(bulletPrefab, transform.position, transform.rotation);
        firework.GetComponent<FireworkBehaviour>().setFireworkSender(transform.parent.gameObject);
    }
}
