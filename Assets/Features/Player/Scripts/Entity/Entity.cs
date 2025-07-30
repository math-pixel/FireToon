using System;
using UnityEngine;

public abstract class Entity: MonoBehaviour
{
    Life life;
    public event Action OnDie;
    public event Action<int> OnTakeDamage;
    
    public bool IsDead => life.Amount <= 0;
    
    public void Init()
    {
        life = new Life(100);
    }

    public virtual void Die()
    {
        if (IsDead) return;
        OnDie?.Invoke();
    }
    
    public virtual void TakeDamage(int damage)
    {
        if (IsDead) return;
        
        life.ReduceLife(damage);
        
        OnTakeDamage?.Invoke(damage);
        
        if (life.Amount <= 0) Die();
    }
    
    public virtual void Heal(int amount)
    {
        if (IsDead) return;
        
        life.AddLife(amount);
    }
}
