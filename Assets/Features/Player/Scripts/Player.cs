
using UnityEngine;

public class Player: Entity
{
    
    Movement movement;
    
    void Start()
    {
        Init();
        movement = gameObject.GetOrAdd<Movement>();
        movement.Init();
        
    }
    
    public override void Die()
    {
        Debug.Log("Player died!");
        base.Die();
    }
}

