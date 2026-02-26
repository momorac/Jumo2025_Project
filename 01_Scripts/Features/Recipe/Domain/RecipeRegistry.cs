using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>레시피에 필요한 재료 항목</summary>
[Serializable]
public class RecipeIngredient
{
    public IngredientType ingredient;
    public int amount = 1;
}

/// <summary>개별 레시피 정의 데이터</summary>
[Serializable]
public class RecipeDefinition
{
    [Header("기본 정보")]
    public RecipeType type;
    public RecipeCategory category;
    public RecipeSubCategory subCategory;
    public string displayName;
    public Sprite icon;

    [Header("재료")]
    public List<RecipeIngredient> ingredients = new List<RecipeIngredient>();

    [Header("조리")]
    [Tooltip("조리 가능한 설비 목록 (복수 지정 가능)")]
    public List<CookingFacilityType> requiredFacilities = new List<CookingFacilityType>();
    public float cookingTime = 3f;

    /// <summary>해당 설비에서 조리 가능한지 확인</summary>
    public bool CanCookAt(CookingFacilityType facility)
    {
        return requiredFacilities.Contains(facility);
    }

    [Header("판매")]
    public int basePrice;

    [Header("버퍼 자원 (밥/김치)")]
    [Tooltip("밥, 김치처럼 미리 만들어두고 소비되는 버퍼 자원인지")]
    public bool isBufferResource;
    [Tooltip("한 번 제작 시 생성되는 수량")]
    public int bufferOutputAmount = 1;

    [Header("중간재료 생성 (김치류)")]
    [Tooltip("제작 시 인벤토리에 추가되는 재료 (None이면 일반 요리)")]
    public IngredientType outputIngredient = IngredientType.None;
}

/// <summary>레시피 정적 데이터 레지스트리 (ScriptableObject)</summary>
[CreateAssetMenu(menuName = "Scriptable Objects/Recipe Registry", fileName = "RecipeRegistry")]
public class RecipeRegistry : ScriptableObject
{
    [SerializeField] private List<RecipeDefinition> entries = new List<RecipeDefinition>();

    private Dictionary<RecipeType, RecipeDefinition> index;
    private Dictionary<RecipeCategory, List<RecipeDefinition>> categoryIndex;
    private Dictionary<RecipeSubCategory, List<RecipeDefinition>> subCategoryIndex;
    private Dictionary<CookingFacilityType, List<RecipeDefinition>> facilityIndex;
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
        index = new Dictionary<RecipeType, RecipeDefinition>(entries.Count);
        categoryIndex = new Dictionary<RecipeCategory, List<RecipeDefinition>>();
        subCategoryIndex = new Dictionary<RecipeSubCategory, List<RecipeDefinition>>();
        facilityIndex = new Dictionary<CookingFacilityType, List<RecipeDefinition>>();

        foreach (var entry in entries)
        {
            index[entry.type] = entry;

            // 카테고리별 인덱스
            if (!categoryIndex.ContainsKey(entry.category))
                categoryIndex[entry.category] = new List<RecipeDefinition>();
            categoryIndex[entry.category].Add(entry);

            // 소분류별 인덱스
            if (!subCategoryIndex.ContainsKey(entry.subCategory))
                subCategoryIndex[entry.subCategory] = new List<RecipeDefinition>();
            subCategoryIndex[entry.subCategory].Add(entry);

            // 조리시설별 인덱스
            foreach (var facility in entry.requiredFacilities)
            {
                if (!facilityIndex.ContainsKey(facility))
                    facilityIndex[facility] = new List<RecipeDefinition>();
                facilityIndex[facility].Add(entry);
            }
        }

        hasBuiltIndex = true;
    }

    /// <summary>타입으로 레시피 정의 조회</summary>
    public RecipeDefinition GetByType(RecipeType type)
    {
        EnsureIndex();
        return index.TryGetValue(type, out var def) ? def : null;
    }

    /// <summary>카테고리별 레시피 목록 조회</summary>
    public IReadOnlyList<RecipeDefinition> GetByCategory(RecipeCategory category)
    {
        EnsureIndex();
        return categoryIndex.TryGetValue(category, out var list) ? list : new List<RecipeDefinition>();
    }

    /// <summary>소분류별 레시피 목록 조회</summary>
    public IReadOnlyList<RecipeDefinition> GetBySubCategory(RecipeSubCategory subCategory)
    {
        EnsureIndex();
        return subCategoryIndex.TryGetValue(subCategory, out var list) ? list : new List<RecipeDefinition>();
    }

    /// <summary>전체 레시피 목록 조회</summary>
    public IReadOnlyList<RecipeDefinition> GetAll()
    {
        return entries;
    }

    /// <summary>레시피 표시 이름 조회</summary>
    public string GetDisplayName(RecipeType type)
    {
        var def = GetByType(type);
        return def?.displayName ?? string.Empty;
    }

    /// <summary>버퍼 자원 레시피 목록 (밥, 김치)</summary>
    public IReadOnlyList<RecipeDefinition> GetBufferRecipes()
    {
        EnsureIndex();
        var result = new List<RecipeDefinition>();
        foreach (var entry in entries)
        {
            if (entry.isBufferResource)
                result.Add(entry);
        }
        return result;
    }

    /// <summary>특정 조리시설에서 사용 가능한 레시피 목록</summary>
    public IReadOnlyList<RecipeDefinition> GetByFacility(CookingFacilityType facility)
    {
        EnsureIndex();
        return facilityIndex.TryGetValue(facility, out var list) ? list : new List<RecipeDefinition>();
    }

    /// <summary>중간재료를 생성하는 레시피 목록 (김치류)</summary>
    public IReadOnlyList<RecipeDefinition> GetIntermediateRecipes()
    {
        EnsureIndex();
        var result = new List<RecipeDefinition>();
        foreach (var entry in entries)
        {
            if (entry.outputIngredient != IngredientType.None)
                result.Add(entry);
        }
        return result;
    }

    private void EnsureIndex()
    {
        if (!hasBuiltIndex)
        {
            BuildIndex();
        }
    }
}
