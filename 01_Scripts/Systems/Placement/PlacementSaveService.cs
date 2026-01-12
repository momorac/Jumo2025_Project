using System.IO;
using UnityEngine;

public static class PlacementSaveService
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "placement.json");

    public static void Save(PlacementSaveData data)
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(Path, json);
#if UNITY_EDITOR
        Debug.Log($"[PlacementSaveService] Saved to {Path}: {json}");
#endif
    }

    public static PlacementSaveData Load()
    {
        try
        {
            if (File.Exists(Path))
            {
                var json = File.ReadAllText(Path);
                var data = JsonUtility.FromJson<PlacementSaveData>(json);
#if UNITY_EDITOR
                Debug.Log($"[PlacementSaveService] Loaded from {Path}: {json}");
#endif
                return data ?? new PlacementSaveData();
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[PlacementSaveService] Load failed: {e.Message}");
#endif
        }
        return new PlacementSaveData();
    }
}
