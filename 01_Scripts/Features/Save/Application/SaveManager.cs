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
        GameLogger.LogVerbose(LogCategory.System, $"[SaveService] Saved to {Path}: {json}");
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
                    GameLogger.LogWarning(LogCategory.System, "[SaveService] Save file is empty, initializing new save.");
                    return InitializeNewSave(config);
                }

                var data = JsonConvert.DeserializeObject<GameMetaData>(json);
#if UNITY_EDITOR
                GameLogger.LogVerbose(LogCategory.System, $"[SaveService] Loaded from {Path}: {json}");
#endif
                return data;
            }
            else
            {
                // 기존에 저장된 파일 없으면 새로운 저장 파일 생성
                GameLogger.LogWarning(LogCategory.System, "[SaveService] Save file does not exist, initializing new save.");
                return InitializeNewSave(config);
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            GameLogger.LogWarning(LogCategory.System, $"[SaveService] Load failed: {e.Message}");
#endif
        }
        return null;
    }

    private static GameMetaData InitializeNewSave(InitialSaveConfig config)
    {
        var placeableMeta = new PlaceableMeta();
        var ingredientMeta = new IngredientMeta();
        var recipeMeta = new RecipeMeta();
        var economyMeta = new EconomyMeta(100);

        if (config == null)
        {
            GameLogger.LogWarning(LogCategory.System, "[SaveManager] InitialSaveConfig is null. Applying default initial state.");
        }
        else
        {
            // ── Economy ────────────────────────────────────────────────────
            economyMeta = new EconomyMeta(config.InitialMoney);

            // ── Placeable ──────────────────────────────────────────────────
            foreach (var facility in config.UnlockedFacilities) placeableMeta.ul_facility.Add(facility);
            foreach (var tile in config.UnlockedTiles) placeableMeta.ul_tile.Add(tile);
            foreach (var decoration in config.UnlockedDecorations) placeableMeta.ul_decoration.Add(decoration);

            // ── Ingredient ─────────────────────────────────────────────────
            ingredientMeta.UnlockedIngredients.Clear();
            foreach (var ingredient in config.UnlockedIngredients) ingredientMeta.UnlockedIngredients.Add(ingredient);
            ingredientMeta.Inventory.Clear();
            foreach (var entry in config.StartingInventory) ingredientMeta.Inventory[entry.type] = entry.amount;

            // ── Recipe ─────────────────────────────────────────────────────
            recipeMeta.UnlockedRecipes.Clear();
            foreach (var recipe in config.UnlockedRecipes) recipeMeta.UnlockedRecipes.Add(recipe);
            recipeMeta.BufferStock.Clear();
            foreach (var entry in config.StartingBufferStock) recipeMeta.BufferStock[entry.type] = entry.amount;
        }

        var newData = new GameMetaData()
        {
            PlacementMeta = null,
            PlaceableMeta = placeableMeta,
            EconomyMeta = economyMeta,
            IngredientMeta = ingredientMeta,
            RecipeMeta = recipeMeta,
        };

        Save(newData);
        return newData;
    }
}
