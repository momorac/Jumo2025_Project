using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 새 게임 시작 시 초기 저장 상태(GameMetaData 전체)를 정의하는 설정 에셋.
/// 인스펙터에서 편집하며, SaveManager가 새 세이브를 생성할 때 적용됩니다.
/// </summary>
[CreateAssetMenu(menuName = "Scriptable Objects/InitialSaveConfig", fileName = "InitialSaveConfig")]
public class InitialSaveConfig : ScriptableObject
{
    // ─── Economy ───────────────────────────────────────────────────────────
    [Header("Economy")]
    [SerializeField] private int startingGold = 100;

    // ─── Placeable ─────────────────────────────────────────────────────────
    [Header("Unlocked Facilities")]
    [SerializeField]
    private List<FacilityType> unlockedFacilities = new()
    {
        FacilityType.Table,
        FacilityType.JumoHouse,
        FacilityType.Sot,
    };

    [Header("Unlocked Tiles")]
    [SerializeField] private List<TileType> unlockedTiles = new();

    [Header("Unlocked Decorations")]
    [SerializeField] private List<DecorationType> unlockedDecorations = new();

    // ─── Ingredient ────────────────────────────────────────────────────────
    [Header("Unlocked Ingredients")]
    [SerializeField]
    private List<IngredientType> unlockedIngredients = new()
    {
        IngredientType.Rice,
        IngredientType.Salt,
    };

    [Header("Starting Ingredient Inventory")]
    [SerializeField] private List<IngredientAmount> startingInventory = new();

    // ─── Recipe ────────────────────────────────────────────────────────────
    [Header("Unlocked Recipes")]
    [SerializeField]
    private List<RecipeType> unlockedRecipes = new()
    {
        RecipeType.WhiteRice,
        RecipeType.MixedGrainRice,
        RecipeType.RadishSoup,
        RecipeType.CabbageKimchi,
    };

    [Header("Starting Buffer Stock")]
    [SerializeField] private List<RecipeAmount> startingBufferStock = new();

    // ─── Properties ────────────────────────────────────────────────────────
    public int StartingGold => startingGold;

    public IReadOnlyList<FacilityType> UnlockedFacilities => unlockedFacilities;
    public IReadOnlyList<TileType> UnlockedTiles => unlockedTiles;
    public IReadOnlyList<DecorationType> UnlockedDecorations => unlockedDecorations;

    public IReadOnlyList<IngredientType> UnlockedIngredients => unlockedIngredients;
    public IReadOnlyList<IngredientAmount> StartingInventory => startingInventory;

    public IReadOnlyList<RecipeType> UnlockedRecipes => unlockedRecipes;
    public IReadOnlyList<RecipeAmount> StartingBufferStock => startingBufferStock;
}

/// <summary>인스펙터용 재료 보유량 항목</summary>
[Serializable]
public struct IngredientAmount
{
    public IngredientType type;
    public int amount;
}

/// <summary>인스펙터용 레시피 버퍼 재고 항목</summary>
[Serializable]
public struct RecipeAmount
{
    public RecipeType type;
    public int amount;
}
