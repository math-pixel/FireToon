using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePlayer : MonoBehaviour
{
    
    public int lifes = 3;
    
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
            StartCoroutine(deadplayer());
        }
    }
    
    private IEnumerator deadplayer()
    {
        gameObject.GetComponent<PlayerMovement>().animator.SetTrigger("die");
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.GameOver();
        // Destroy(gameObject);
    } 

    public void IncreaseLife(int amount)
    {
        Debug.Log(gameObject.name + " has " + lifes + " live(s)");
        lifes += amount;
    }
}
