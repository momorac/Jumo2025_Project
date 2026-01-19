using System.Collections.Generic;
using UnityEngine;

public interface IPlacementController
{
    bool CanPlace(Placeable type);
    GameObject Place(Placeable type, Vector3 pos, Quaternion rot);

    public Vector2Int GetGridSize();

    public IReadOnlyCollection<FacilityType> GetAvailableFacilities();
    public IReadOnlyCollection<TileType> GetAvailableTiles();
    public IReadOnlyCollection<DecorationType> GetAvailableDecorations();

    public GameObject GetGameObjectPrefab(FacilityType facilityType);
    public GameObject GetGameObjectPrefab(TileType tileType);
    public GameObject GetGameObjectPrefab(DecorationType decorationType);

    public GameObject GetUiIconPrefab(FacilityType facilityType);
    public GameObject GetUiIconPrefab(TileType tileType);
    public GameObject GetUiIconPrefab(DecorationType decorationType);
}
