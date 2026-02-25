using System.Collections.Generic;
using UnityEngine;

/// <summary>레시피 관리 서비스 (해금, 조리, 버퍼 관리)</summary>
public class RecipeService
{
    private readonly RecipeData data;
    private readonly RecipeRegistry registry;

    public RecipeService(RecipeData data, RecipeRegistry registry)
    {
        this.data = data;
        this.registry = registry;
    }

    #region 해금 관리

    /// <summary>레시피 해금 여부 확인</summary>
    public bool IsUnlocked(RecipeType type)
    {
        return data.UnlockedRecipes.Contains(type);
    }

    /// <summary>레시피 해금</summary>
    public bool Unlock(RecipeType type)
    {
        if (data.UnlockedRecipes.Contains(type))
            return false;

        data.UnlockedRecipes.Add(type);
        Debug.Log($"<color=green>Recipe unlocked: {type}</color>");
        return true;
    }

    /// <summary>해금된 레시피 목록 반환</summary>
    public IReadOnlyCollection<RecipeType> GetUnlockedRecipes()
    {
        return data.UnlockedRecipes;
    }

    /// <summary>특정 카테고리에서 해금된 레시피 목록</summary>
    public List<RecipeDefinition> GetUnlockedByCategory(RecipeCategory category)
    {
        var result = new List<RecipeDefinition>();
        foreach (var def in registry.GetByCategory(category))
        {
            if (IsUnlocked(def.type))
                result.Add(def);
        }
        return result;
    }

    /// <summary>특정 소분류에서 해금된 레시피 목록</summary>
    public List<RecipeDefinition> GetUnlockedBySubCategory(RecipeSubCategory subCategory)
    {
        var result = new List<RecipeDefinition>();
        foreach (var def in registry.GetBySubCategory(subCategory))
        {
            if (IsUnlocked(def.type))
                result.Add(def);
        }
        return result;
    }

    #endregion

    #region 조리 가능 여부

    /// <summary>재료가 충분한지 확인</summary>
    public bool CanCook(RecipeType type)
    {
        var def = registry.GetByType(type);
        if (def == null) return false;

        foreach (var req in def.ingredients)
        {
            if (!App.IngredientService.HasAmount(req.ingredient, req.amount))
                return false;
        }
        return true;
    }

    /// <summary>해금 + 재료 모두 충족하는지 확인</summary>
    public bool CanCookUnlocked(RecipeType type)
    {
        return IsUnlocked(type) && CanCook(type);
    }

    /// <summary>현재 재료로 조리 가능한 레시피 목록</summary>
    public List<RecipeDefinition> GetCookableRecipes()
    {
        var result = new List<RecipeDefinition>();
        foreach (var def in registry.GetAll())
        {
            if (IsUnlocked(def.type) && CanCook(def.type))
                result.Add(def);
        }
        return result;
    }

    /// <summary>특정 소분류에서 조리 가능한 레시피 목록</summary>
    public List<RecipeDefinition> GetCookableBySubCategory(RecipeSubCategory subCategory)
    {
        var result = new List<RecipeDefinition>();
        foreach (var def in registry.GetBySubCategory(subCategory))
        {
            if (IsUnlocked(def.type) && CanCook(def.type))
                result.Add(def);
        }
        return result;
    }

    #endregion

    #region 조리 실행

    /// <summary>레시피 조리 (재료 소비 + 결과물 생성)</summary>
    public bool Cook(RecipeType type)
    {
        var def = registry.GetByType(type);
        if (def == null)
        {
            Debug.LogWarning($"Recipe not found: {type}");
            return false;
        }

        if (!CanCook(type))
        {
            Debug.LogWarning($"Cannot cook {type}: insufficient ingredients");
            return false;
        }

        // 재료 소비
        var requirements = new Dictionary<IngredientType, int>();
        foreach (var req in def.ingredients)
        {
            requirements[req.ingredient] = req.amount;
        }

        if (!App.IngredientService.ConsumeMultiple(requirements))
            return false;

        // 결과물 처리
        if (def.outputIngredient != IngredientType.None)
        {
            // 중간재료 생성 (김치류)
            App.IngredientService.Add(def.outputIngredient, 1);
            Debug.Log($"<color=cyan>Cooked {type} → produced {def.outputIngredient}</color>");
        }

        if (def.isBufferResource)
        {
            // 버퍼 자원 추가 (밥/김치)
            AddToBuffer(type, def.bufferOutputAmount);
            Debug.Log($"<color=cyan>Cooked {type} → buffer +{def.bufferOutputAmount}</color>");
        }
        else if (def.outputIngredient == IngredientType.None)
        {
            // 일반 요리 완성
            Debug.Log($"<color=cyan>Cooked {type} (ready to serve)</color>");
        }

        return true;
    }

    #endregion

    #region 버퍼 자원 관리

    /// <summary>버퍼 재고 조회</summary>
    public int GetBufferStock(RecipeType type)
    {
        return data.BufferStock.TryGetValue(type, out int amount) ? amount : 0;
    }

    /// <summary>버퍼에 추가</summary>
    public void AddToBuffer(RecipeType type, int amount)
    {
        if (amount <= 0) return;

        if (!data.BufferStock.ContainsKey(type))
            data.BufferStock[type] = 0;

        data.BufferStock[type] += amount;
        Debug.Log($"<color=green>Buffer added: {type} +{amount} (Total: {data.BufferStock[type]})</color>");
    }

    /// <summary>버퍼에서 소비</summary>
    public bool ConsumeFromBuffer(RecipeType type, int amount = 1)
    {
        int current = GetBufferStock(type);
        if (current < amount)
        {
            Debug.LogWarning($"Not enough buffer stock for {type}: have {current}, need {amount}");
            return false;
        }

        data.BufferStock[type] -= amount;
        Debug.Log($"<color=yellow>Buffer consumed: {type} -{amount} (Remaining: {data.BufferStock[type]})</color>");
        return true;
    }

    /// <summary>버퍼 재고 충분한지 확인</summary>
    public bool HasBufferStock(RecipeType type, int amount = 1)
    {
        return GetBufferStock(type) >= amount;
    }

    #endregion

    #region 레지스트리 조회 위임

    /// <summary>레시피 정의 조회</summary>
    public RecipeDefinition GetDefinition(RecipeType type)
    {
        return registry.GetByType(type);
    }

    /// <summary>카테고리별 레시피 목록</summary>
    public IReadOnlyList<RecipeDefinition> GetByCategory(RecipeCategory category)
    {
        return registry.GetByCategory(category);
    }

    /// <summary>소분류별 레시피 목록</summary>
    public IReadOnlyList<RecipeDefinition> GetBySubCategory(RecipeSubCategory subCategory)
    {
        return registry.GetBySubCategory(subCategory);
    }

    /// <summary>전체 레시피 목록</summary>
    public IReadOnlyList<RecipeDefinition> GetAllDefinitions()
    {
        return registry.GetAll();
    }

    #endregion
}
