using System.Collections.Generic;
using UnityEngine;

public interface IPlacementController
{
    bool CanPlace(Placeable type);
    GameObject Place(Placeable type, Vector3 pos, Quaternion rot);
    void StartPlacing(Placeable placeable);

    public IReadOnlyCollection<FacilityType> GetAvailableFacilities();
    public IReadOnlyCollection<TileType> GetAvailableTiles();
    public IReadOnlyCollection<DecorationType> GetAvailableDecorations();

    public GameObject GetGameObjectPrefab(Placeable type);
    public GameObject GetGameObjectPrefab(FacilityType facilityType);
    public GameObject GetGameObjectPrefab(TileType tileType);
    public GameObject GetGameObjectPrefab(DecorationType decorationType);

    public Sprite GetUiIcon(FacilityType facilityType);
    public Sprite GetUiIcon(TileType tileType);
    public Sprite GetUiIcon(DecorationType decorationType);
}
