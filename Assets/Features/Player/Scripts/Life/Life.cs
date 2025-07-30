public class Life
{
    public int Amount { get; set; }

    public int MaxLife { get; set; }

    public Life(int maxLife)
    {
        MaxLife = maxLife;
        Amount = maxLife;
    }
    
    public void ReduceLife(int damage)
    {
        Amount -= damage;
    }
    public void AddLife(int amount)
    {
        Amount += amount;
    }
}