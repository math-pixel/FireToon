using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class LifePlayer : MonoBehaviour
{
    [Header("Configuration")]
    public PlayerConfig playerConfig;
    
    private int lifes;
    
    void Start()
    {
        lifes = playerConfig != null ? playerConfig.startingLifes : 3;
    }

    public void ReduceLife(int amount)
    {
        lifes -= amount;
        Debug.Log($"{gameObject.name} has {lifes} live(s)");
        
        if (lifes <= 0)
        {
            StartCoroutine(DeadPlayer());
        }
    }
    
    private IEnumerator DeadPlayer()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement?.animator != null)
        {
            playerMovement.animator.SetTrigger("die");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        PlayerInput playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            GameManager.Instance.PlayerDead(playerInput);
        }
        
        gameObject.SetActive(false);
    }

    public void IncreaseLife(int amount)
    {
        lifes += amount;
        Debug.Log($"{gameObject.name} has {lifes} live(s)");
    }
    
    public int GetCurrentLifes()
    {
        return lifes;
    }
}