using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Facility Registry", fileName = "PlacementRegistry")]
public class PlacementRegistry : ScriptableObject
{
    [Serializable]
    public struct DecorationEntry
    {
        public string id;
        public GameObject prefab;
    }

    [Serializable]
    public struct FacilityEntry
    {
        public FacilityType type;
        public GameObject prefab;
    }

    [SerializeField] private List<FacilityEntry> facilityEntries = new List<FacilityEntry>();
    [SerializeField] private List<DecorationEntry> decorationEntries = new List<DecorationEntry>();

    public IReadOnlyList<FacilityEntry> FacilityEntries => facilityEntries;
    public IReadOnlyList<DecorationEntry> DecorationEntries => decorationEntries;

    public GameObject GetPrefab(FacilityType type)
    {
        for (int i = 0; i < facilityEntries.Count; i++)
        {
            if (facilityEntries[i].type == type) return facilityEntries[i].prefab;
        }
        return null;
    }

    public GameObject GetPrefab(string id)
    {
        for (int i = 0; i < decorationEntries.Count; i++)
        {
            if (decorationEntries[i].id == id) return decorationEntries[i].prefab;
        }
        return null;
    }

}

public enum PlacementType
{
    Tile,
    Facility,
    Decoration
}

