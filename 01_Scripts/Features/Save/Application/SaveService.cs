using System.IO;
using UnityEngine;

public static class SaveService
{
    private static string Path => System.IO.Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(GameMetaData data)
    {
        var json = JsonUtility.ToJson(data);
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
                var data = JsonUtility.FromJson<GameMetaData>(json);
#if UNITY_EDITOR
                Debug.Log($"[SaveService] Loaded from {Path}: {json}");
#endif
                return data;
            }
            else
            {
                // 기존에 저장된 파일 없으면 새로운 저장 파일 생성
                var newData = new GameMetaData();
                Save(newData);
                return newData;
            }
        }
        catch (System.Exception e)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"[SaveService] Load failed: {e.Message}");
#endif
        }
        return new GameMetaData();
    }
}
