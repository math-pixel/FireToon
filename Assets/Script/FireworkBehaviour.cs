using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FireworkBehaviour : MonoBehaviour
{
    
    [Header("Firework Settings")]
    public float speed = 1f;
    public int maxBounce = 3;
    public VFX_Firework explosion;
    
    private GameObject fireworkSender;
    private int currentBounce = 0;
    private Rigidbody rb;
    private Vector3 lateVelocity;
    private Vector3 directionBullet;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        directionBullet = transform.forward;
    }

    private void Update()
    {
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        // Go forward   
        rb.velocity = transform.forward * speed * Time.deltaTime;
        lateVelocity = rb.velocity;
        
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
        
    }

    private void OnCollisionEnter(Collision other)
    {
        // test if ist not sender or another firework
        if (fireworkSender.name != other.gameObject.name && other.gameObject.name != gameObject.name)
        {
            
            // if  touch reduce life
            Debug.Log(other.gameObject.name + " collided with " + gameObject.name);
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<LifePlayer>().ReduceLife(1);
                explosion.play();
                Destroy(gameObject);
            }

            // bounce
            if (currentBounce >= maxBounce)
            {
                explosion.play();
                Destroy(gameObject);
            }
            else
            {
                float curSpeed = lateVelocity.magnitude;
                Vector3 direction = Vector3.Reflect(lateVelocity.normalized, other.contacts[0].normal);
                directionBullet = direction * curSpeed * Time.deltaTime;

                transform.rotation = Quaternion.LookRotation(direction);
                
                float angle = Vector3.Angle(direction, directionBullet);
                currentBounce++;
            }
        
            
        }
    }
}
