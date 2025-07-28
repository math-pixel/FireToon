using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Life
{
    public int Amount { get; set; }

    public int MaxLife { get; set; }

    public void ReduceLife(int damage)
    {
        Amount -= damage;
    }
    public void AddLife(int amount)
    {
        Amount += amount;
    }
}