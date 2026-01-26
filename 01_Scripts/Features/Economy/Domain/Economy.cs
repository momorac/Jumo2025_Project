using System;

public class Economy
{
    public int Money { get; private set; }

    public Economy(int initialMoney)
    {
        Money = initialMoney;
    }

    public void Add(int amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        Money += amount;
    }

    public bool TrySpend(int amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (Money < amount) return false;
        Money -= amount;
        return true;
    }
}
