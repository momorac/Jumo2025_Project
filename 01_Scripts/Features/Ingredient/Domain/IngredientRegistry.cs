using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>개별 재료 정의 데이터</summary>
[Serializable]
public class IngredientDefinition
{
    public IngredientType type;
    public IngredientCategory category;
    public string displayName;
    public Sprite icon;
    public int basePrice;
    public bool isFromFarming;  // 농사로 획득하는 재료 여부
}

/// <summary>재료 정적 데이터 레지스트리 (ScriptableObject)</summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Ingredient Registry", fileName = "IngredientRegistry")]
public class IngredientRegistry : ScriptableObject
{
    [SerializeField] private List<IngredientDefinition> entries = new List<IngredientDefinition>();

    private Dictionary<IngredientType, IngredientDefinition> index;
    private Dictionary<IngredientCategory, List<IngredientDefinition>> categoryIndex;
    private bool hasBuiltIndex = false;

    private void OnEnable()
    {
        BuildIndex();
    }

    private void OnValidate()
    {
        BuildIndex();
    }

    private void BuildIndex()
    {
        index = new Dictionary<IngredientType, IngredientDefinition>(entries.Count);
        categoryIndex = new Dictionary<IngredientCategory, List<IngredientDefinition>>();

        foreach (var entry in entries)
        {
            index[entry.type] = entry;

            if (!categoryIndex.ContainsKey(entry.category))
            {
                categoryIndex[entry.category] = new List<IngredientDefinition>();
            }
            categoryIndex[entry.category].Add(entry);
        }

        hasBuiltIndex = true;
    }

    /// <summary>타입으로 재료 정의 조회</summary>
    public IngredientDefinition GetByType(IngredientType type)
    {
        EnsureIndex();
        return index.TryGetValue(type, out var def) ? def : null;
    }

    /// <summary>카테고리별 재료 목록 조회</summary>
    public IReadOnlyList<IngredientDefinition> GetByCategory(IngredientCategory category)
    {
        EnsureIndex();
        return categoryIndex.TryGetValue(category, out var list) ? list : new List<IngredientDefinition>();
    }

    /// <summary>전체 재료 목록 조회</summary>
    public IReadOnlyList<IngredientDefinition> GetAll()
    {
        return entries;
    }

    /// <summary>재료 표시 이름 조회</summary>
    public string GetDisplayName(IngredientType type)
    {
        var def = GetByType(type);
        return def?.displayName ?? string.Empty;
    }

    /// <summary>재료 아이콘 조회</summary>
    public Sprite GetIcon(IngredientType type)
    {
        var def = GetByType(type);
        return def?.icon;
    }

    /// <summary>재료 기본 가격 조회</summary>
    public int GetBasePrice(IngredientType type)
    {
        var def = GetByType(type);
        return def?.basePrice ?? 0;
    }

    /// <summary>농사 재료 여부 확인</summary>
    public bool IsFromFarming(IngredientType type)
    {
        var def = GetByType(type);
        return def?.isFromFarming ?? false;
    }

    private void EnsureIndex()
    {
        if (!hasBuiltIndex)
        {
            BuildIndex();
        }
    }
}
