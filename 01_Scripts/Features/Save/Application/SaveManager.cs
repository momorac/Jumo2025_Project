using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveManager
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(GameMetaData data)
    {
        var json = JsonConvert.SerializeObject(data);
        File.WriteAllText(Path, json);
#if UNITY_EDITOR
        Debug.Log($"[SaveService] Saved to {Path}: {json}");
#endif
    }

    public static GameMetaData Load(InitialSaveConfig config)
    {
        try
        {
            if (File.Exists(Path))
            {
                var json = File.ReadAllText(Path);
                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning("[SaveService] Save file is empty, initializing new save.");
                    return InitializeNewSave(config);
                }

                var data = JsonConvert.DeserializeObject<GameMetaData>(json);
#if UNITY_EDITOR
                Debug.Log($"[SaveService] Loaded from {Path}: {json}");
#endif
                return data;
            }
            else
            {
                // 기존에 저장된 파일 없으면 새로운 저장 파일 생성
                Debug.LogWarning("[SaveService] Save file does not exist, initializing new save.");
                return InitializeNewSave(config);
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[SaveService] Load failed: {e.Message}");
#endif
        }
        return null;
    }

    private static GameMetaData InitializeNewSave(InitialSaveConfig config)
    {
        var placeableData = new PlaceableData();
        var ingredientData = new IngredientData();
        var recipeData = new RecipeData();
        var economy = new Economy(100);

        if (config == null)
        {
            Debug.LogWarning("[SaveManager] InitialSaveConfig is null. Applying default initial state.");
        }
        else
        {
            // ── Economy ────────────────────────────────────────────────────
            economy = new Economy(config.StartingGold);

            // ── Placeable ──────────────────────────────────────────────────
            foreach (var facility in config.UnlockedFacilities) placeableData.ul_facility.Add(facility);
            foreach (var tile in config.UnlockedTiles) placeableData.ul_tile.Add(tile);
            foreach (var decoration in config.UnlockedDecorations) placeableData.ul_decoration.Add(decoration);

            // ── Ingredient ─────────────────────────────────────────────────
            ingredientData.UnlockedIngredients.Clear();
            foreach (var ingredient in config.UnlockedIngredients) ingredientData.UnlockedIngredients.Add(ingredient);
            ingredientData.Inventory.Clear();
            foreach (var entry in config.StartingInventory) ingredientData.Inventory[entry.type] = entry.amount;

            // ── Recipe ─────────────────────────────────────────────────────
            recipeData.UnlockedRecipes.Clear();
            foreach (var recipe in config.UnlockedRecipes) recipeData.UnlockedRecipes.Add(recipe);
            recipeData.BufferStock.Clear();
            foreach (var entry in config.StartingBufferStock) recipeData.BufferStock[entry.type] = entry.amount;
        }

        var newData = new GameMetaData()
        {
            PlacementData = null,
            PlaceableData = placeableData,
            EconomyData = economy,
            IngredientData = ingredientData,
            RecipeData = recipeData,
        };

        Save(newData);
        return newData;
    }
}
