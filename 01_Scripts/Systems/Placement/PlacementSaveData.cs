using System.Collections.Generic;

[System.Serializable]
public class PlacementRecord
{
    // Stable numeric/hash identifier for the prefab
    public int id;
    public int x;
    public int y;
}

[System.Serializable]
public class PlacementSaveData
{
    public List<PlacementRecord> records = new();
}
