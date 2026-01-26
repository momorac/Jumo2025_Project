using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveService
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

    public static GameMetaData Load()
    {
        try
        {
            if (File.Exists(Path))
            {
                var json = File.ReadAllText(Path);
                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogWarning("[SaveService] Save file is empty, initializing new save.");
                    return InitializeNewSave();
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
                return InitializeNewSave();
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

    private static GameMetaData InitializeNewSave()
    {
        var newData = new GameMetaData()
        {
            PlacementData = null,
            PlaceableData = new PlaceableData(),
            EconomyData = new Economy(100)
        };

        Save(newData);
        return newData;
    }
}
