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
        public FacilityComponent component;
    }

    [Serializable]
    public struct FacilityEntry
    {
        public FacilityType type;
        public FacilityComponent component;
    }

    [SerializeField] private List<FacilityEntry> facilityEntries = new List<FacilityEntry>();
    [SerializeField] private List<DecorationEntry> decorationEntries = new List<DecorationEntry>();

    public IReadOnlyList<FacilityEntry> FacilityEntries => facilityEntries;
    public IReadOnlyList<DecorationEntry> DecorationEntries => decorationEntries;

    public GameObject GetGameObjectPrefab(FacilityType type)
    {
        for (int i = 0; i < facilityEntries.Count; i++)
        {
            if (facilityEntries[i].type == type)
            {
                return facilityEntries[i].component.gameObjectPrefab;
            }
        }
        return null;
    }

    public GameObject GetGameObjectPrefab(string id)
    {
        for (int i = 0; i < decorationEntries.Count; i++)
        {
            if (decorationEntries[i].id == id) return decorationEntries[i].component.gameObjectPrefab;
        }
        return null;
    }

    public GameObject GetUiIconPrefab(FacilityType type)
    {
        for (int i = 0; i < facilityEntries.Count; i++)
        {
            if (facilityEntries[i].type == type)
            {
                return facilityEntries[i].component.UiIconPrefab;
            }
        }
        return null;
    }

    public GameObject GetUiIconPrefab(string id)
    {
        for (int i = 0; i < decorationEntries.Count; i++)
        {
            if (decorationEntries[i].id == id) return decorationEntries[i].component.UiIconPrefab;
        }
        return null;
    }

}

[Serializable]
public class FacilityComponent
{
    public GameObject gameObjectPrefab;
    public GameObject UiIconPrefab;
}

public enum PlacementType
{
    Tile,
    Facility,
    Decoration
}

