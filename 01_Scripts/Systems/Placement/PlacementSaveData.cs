using System.Collections.Generic;

[System.Serializable]
public class PlacementRecord
{
    public string prefab; // prefab identifier (name)
    public int x;
    public int y;
}

[System.Serializable]
public class PlacementSaveData
{
    public List<PlacementRecord> records = new();
}
