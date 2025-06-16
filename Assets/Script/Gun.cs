using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    
    public GameObject bulletPrefab;
    
    private GameObject bullet;
    
    // Start is called before the first frame update
    void Start()
    {
        setUpBullet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        GameObject firework = Instantiate(bullet, transform.position, transform.rotation);
        firework.GetComponent<FireworkBehaviour>().setFireworkSender(transform.parent.gameObject);
    }

    private void setUpBullet()
    {
        // for set up different type of bullet from an event listener of the player power up taken
        bullet = bulletPrefab;
    }
}
