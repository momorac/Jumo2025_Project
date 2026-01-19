using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacementComponent
{
    public GameObject gameObjectPrefab;
    public GameObject UiIconPrefab;
}

[CreateAssetMenu(menuName = "Settings/Facility Registry", fileName = "PlacementRegistry")]
public class PlacementRegistry : ScriptableObject
{
    [Serializable]
    public struct FacilityEntry
    {
        public FacilityType type;
        public PlacementComponent component;
    }

    [Serializable]
    public struct TileEntry
    {
        public TileType type;
        public PlacementComponent component;
    }

    [Serializable]
    public struct DecorationEntry
    {
        public DecorationType type;
        public PlacementComponent component;
    }


    [SerializeField] private List<FacilityEntry> facilityEntries = new List<FacilityEntry>();
    [SerializeField] private List<TileEntry> tileEntries = new List<TileEntry>();
    [SerializeField] private List<DecorationEntry> decorationEntries = new List<DecorationEntry>();

    private Dictionary<FacilityType, PlacementComponent> facilityIndex;
    private Dictionary<TileType, PlacementComponent> tileIndex;
    private Dictionary<DecorationType, PlacementComponent> decorationIndex;
    private bool hasBuiltIndexes = false;

    private void OnEnable()
    {
        BuildIndexes();
    }

    private void OnValidate()
    {
        BuildIndexes();
    }

    private void BuildIndexes()
    {
        facilityIndex = new Dictionary<FacilityType, PlacementComponent>(facilityEntries.Count);
        for (int i = 0; i < facilityEntries.Count; i++)
        {
            facilityIndex[facilityEntries[i].type] = facilityEntries[i].component;
        }

        tileIndex = new Dictionary<TileType, PlacementComponent>(tileEntries.Count);
        for (int i = 0; i < tileEntries.Count; i++)
        {
            tileIndex[tileEntries[i].type] = tileEntries[i].component;
        }

        decorationIndex = new Dictionary<DecorationType, PlacementComponent>(decorationEntries.Count);
        for (int i = 0; i < decorationEntries.Count; i++)
        {
            decorationIndex[decorationEntries[i].type] = decorationEntries[i].component;
        }

        hasBuiltIndexes = true;
    }

    public GameObject GetGameObjectPrefab(FacilityType facilityType)
    {
        if (facilityIndex != null && facilityIndex.TryGetValue(facilityType, out var component))
        {
            return component.gameObjectPrefab;
        }
        return null;
    }
    public GameObject GetGameObjectPrefab(TileType tileType)
    {
        if (tileIndex != null && tileIndex.TryGetValue(tileType, out var component))
        {
            return component.gameObjectPrefab;
        }
        return null;
    }
    public GameObject GetGameObjectPrefab(DecorationType decorationType)
    {
        if (decorationIndex != null && decorationIndex.TryGetValue(decorationType, out var component))
        {
            return component.gameObjectPrefab;
        }
        return null;
    }

    public GameObject GetUiIconPrefab(FacilityType facilityType)
    {
        if (facilityIndex != null && facilityIndex.TryGetValue(facilityType, out var component))
        {
            return component.UiIconPrefab;
        }
        return null;
    }
    public GameObject GetUiIconPrefab(TileType tileType)
    {
        if (tileIndex != null && tileIndex.TryGetValue(tileType, out var component))
        {
            return component.UiIconPrefab;
        }
        return null;
    }
    public GameObject GetUiIconPrefab(DecorationType decorationType)
    {
        if (decorationIndex != null && decorationIndex.TryGetValue(decorationType, out var component))
        {
            return component.UiIconPrefab;
        }
        return null;
    }
}

