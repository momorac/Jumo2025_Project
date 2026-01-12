using System;

public class EconomyService
{
    private readonly Wallet wallet;

    public event Action<int> OnMoneyChanged;

    public EconomyService(Wallet wallet)
    {
        this.wallet = wallet;
    }

    public void AddIncome(int amount)
    {
        var before = wallet.Money;
        wallet.Add(amount);
        if (wallet.Money != before) OnMoneyChanged?.Invoke(wallet.Money);
    }

    public bool TrySpend(int amount)
    {
        var before = wallet.Money;
        var ok = wallet.TrySpend(amount);
        if (ok && wallet.Money != before) OnMoneyChanged?.Invoke(wallet.Money);
        return ok;
    }

    public int GetMoney() => wallet.Money;
}
