using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePlayer : MonoBehaviour
{
    
    public int lifes = 3;
    public GameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReduceLife(int amount)
    {
        lifes -= amount;
        Debug.Log(gameObject.name + " has " + lifes + " live(s)");
        if (lifes <= 0)
        {
            Destroy(gameObject);
            gameManager.GameOver();
        }
    }

    public void IncreaseLife(int amount)
    {
        Debug.Log(gameObject.name + " has " + lifes + " live(s)");
        lifes += amount;
    }
}
