using System.IO;
using UnityEngine;

public static class SaveService
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(MetaGameData data)
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(Path, json);
#if UNITY_EDITOR
        Debug.Log($"[SaveService] Saved to {Path}: {json}");
#endif
    }

    public static MetaGameData Load()
    {
        try
        {
            if (File.Exists(Path))
            {
                var json = File.ReadAllText(Path);
                var data = JsonUtility.FromJson<MetaGameData>(json);
#if UNITY_EDITOR
                Debug.Log($"[SaveService] Loaded from {Path}: {json}");
#endif
                return data ?? new MetaGameData();
            }
            else
            {
                // 새로운 저장 파일 생성
                var newData = new MetaGameData();
                Save(newData);
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[SaveService] Load failed: {e.Message}");
#endif
        }
        return new MetaGameData();
    }
}
