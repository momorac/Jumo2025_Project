using System;

public class Wallet
{
    public int Money { get; private set; }

    public Wallet(int initialMoney = 0)
    {
        if (initialMoney < 0) throw new ArgumentOutOfRangeException(nameof(initialMoney));
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
