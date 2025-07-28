
using UnityEngine;

public class Player: Entity
{
    public override void Die()
    {
        Debug.Log("Player died!");
        base.Die();
    }
}

