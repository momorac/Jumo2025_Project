using System.Collections.Generic;
using UnityEngine;

/// <summary>재료 관리 서비스 (해금, 보유량, 구매)</summary>
public class IngredientService
{
    private readonly IngredientData data;
    private readonly IngredientRegistry registry;

    public IngredientService(IngredientData data, IngredientRegistry registry)
    {
        this.data = data;
        this.registry = registry;
    }

    #region 해금 관리

    /// <summary>재료 해금 여부 확인</summary>
    public bool IsUnlocked(IngredientType type)
    {
        return data.UnlockedIngredients.Contains(type);
    }

    /// <summary>재료 해금</summary>
    public bool Unlock(IngredientType type)
    {
        if (data.UnlockedIngredients.Contains(type))
            return false;

        data.UnlockedIngredients.Add(type);
        Debug.Log($"<color=green>Ingredient unlocked: {type}</color>");
        return true;
    }

    /// <summary>해금된 재료 목록 반환</summary>
    public IReadOnlyCollection<IngredientType> GetUnlockedIngredients()
    {
        return data.UnlockedIngredients;
    }

    #endregion

    #region 보유량 관리

    /// <summary>재료 보유량 조회</summary>
    public int GetAmount(IngredientType type)
    {
        return data.Inventory.TryGetValue(type, out int amount) ? amount : 0;
    }

    /// <summary>재료 추가</summary>
    public void Add(IngredientType type, int amount)
    {
        if (amount <= 0) return;

        if (!data.Inventory.ContainsKey(type))
        {
            data.Inventory[type] = 0;
        }
        data.Inventory[type] += amount;

        Debug.Log($"<color=green>Ingredient added: {type} x{amount} (Total: {data.Inventory[type]})</color>");
    }

    /// <summary>재료 소비 (성공 시 true)</summary>
    public bool Consume(IngredientType type, int amount)
    {
        if (amount <= 0) return true;

        int current = GetAmount(type);
        if (current < amount)
        {
            Debug.LogWarning($"Not enough {type}: have {current}, need {amount}");
            return false;
        }

        data.Inventory[type] -= amount;
        Debug.Log($"<color=yellow>Ingredient consumed: {type} x{amount} (Remaining: {data.Inventory[type]})</color>");
        return true;
    }

    /// <summary>여러 재료 동시 소비 (모두 충분할 때만 소비)</summary>
    public bool ConsumeMultiple(Dictionary<IngredientType, int> requirements)
    {
        // 먼저 모든 재료가 충분한지 확인
        foreach (var req in requirements)
        {
            if (GetAmount(req.Key) < req.Value)
            {
                Debug.LogWarning($"Not enough {req.Key}: have {GetAmount(req.Key)}, need {req.Value}");
                return false;
            }
        }

        // 모두 충분하면 소비
        foreach (var req in requirements)
        {
            data.Inventory[req.Key] -= req.Value;
        }

        return true;
    }

    /// <summary>특정 수량 보유 여부 확인</summary>
    public bool HasAmount(IngredientType type, int amount)
    {
        return GetAmount(type) >= amount;
    }

    #endregion

    #region 상점 구매

    /// <summary>구매 가능 여부 확인 (골드 + 해금)</summary>
    public bool CanPurchase(IngredientType type, int quantity = 1)
    {
        if (!IsUnlocked(type))
            return false;

        int totalPrice = registry.GetBasePrice(type) * quantity;
        return App.EconomyService.CanAfford(totalPrice);
    }

    /// <summary>재료 구매 (골드 차감 + 보유량 증가)</summary>
    public bool Purchase(IngredientType type, int quantity = 1)
    {
        if (!CanPurchase(type, quantity))
            return false;

        int totalPrice = registry.GetBasePrice(type) * quantity;

        if (!App.EconomyService.TrySpend(totalPrice))
            return false;

        Add(type, quantity);
        Debug.Log($"<color=cyan>Purchased {type} x{quantity} for {totalPrice} gold</color>");
        return true;
    }

    #endregion

    #region 레지스트리 조회 위임

    /// <summary>재료 정의 조회</summary>
    public IngredientDefinition GetDefinition(IngredientType type)
    {
        return registry.GetByType(type);
    }

    /// <summary>카테고리별 재료 목록</summary>
    public IReadOnlyList<IngredientDefinition> GetByCategory(IngredientCategory category)
    {
        return registry.GetByCategory(category);
    }

    /// <summary>전체 재료 목록</summary>
    public IReadOnlyList<IngredientDefinition> GetAllDefinitions()
    {
        return registry.GetAll();
    }

    #endregion
}
