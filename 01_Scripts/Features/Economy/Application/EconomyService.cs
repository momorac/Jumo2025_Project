using System;

public class EconomyService
{
    private readonly Economy economy;

    public event Action<int> OnMoneyChanged;

    public EconomyService(int initialMoney)
    {
        economy = new Economy(initialMoney);
        OnMoneyChanged?.Invoke(economy.Money);
    }

    public void AddIncome(int amount)
    {
        var before = economy.Money;
        economy.Add(amount);
        if (economy.Money != before) OnMoneyChanged?.Invoke(economy.Money);
    }

    public bool TrySpend(int amount)
    {
        var before = economy.Money;
        var ok = economy.TrySpend(amount);
        if (ok && economy.Money != before) OnMoneyChanged?.Invoke(economy.Money);
        return ok;
    }

    public int GetMoney() => economy.Money;

    public Economy GetEconomyData() => economy;
}
