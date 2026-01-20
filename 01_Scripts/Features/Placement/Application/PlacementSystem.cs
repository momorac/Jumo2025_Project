using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

[Serializable]
public class BuildingRoot
{
    public PlaceableType type;
    public Transform root;
}

public class PlacementSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridSystem grid;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlacementRegistry registry;

    [Header("Prefabs")]
    [SerializeField] private PlaceableObject placePrefab;
    // [SerializeField] private List<Placeable> placeablePrefabs = new List<Placeable>();

    [Header("Parents by Type")]
    [SerializeField] private List<BuildingRoot> buildingRoots = new List<BuildingRoot>();

    [Header("Raycast")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rayMaxDistance = 200f;


    private PlaceableObject previewCell;
    private Int2 currentCell = new Int2(-1, -1);
    private bool isPlaceable = true;

    // Save/Load state
    private readonly Dictionary<PlaceableType, Transform> rootIndex = new Dictionary<PlaceableType, Transform>();
    private bool isLoading;
    private bool isPlacing = false;


    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Map building type -> parent root
        rootIndex.Clear();
        foreach (var br in buildingRoots)
        {
            if (br != null && br.root != null)
            {
                rootIndex[br.type] = br.root;
            }
        }

        LoadPlacementData();
    }

    void Update()
    {
        if (!isPlacing)
            return;

        UpdatePreview();
        if (isPlaceable)
        {
            HandlePlacement();
        }
    }

    #region Public Methods
    public void StartPlacing(Placeable placeable, GameObject prefab)
    {
        placePrefab = prefab.GetComponent<PlaceableObject>();
        placePrefab.Bind(placeable);

        if (placePrefab != null)
        {
            previewCell = Instantiate(placePrefab);
        }

        isPlacing = true;
    }

    #endregion

    private void UpdatePreview()
    {
        if (mainCamera == null || grid == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, rayMaxDistance, groundMask))
        {
            Vector3 worldPos = hit.point;
            Int2 _cell = grid.WorldToGrid(worldPos);

            if (_cell.Equals(currentCell))
                return;

            if (grid.IsInBounds(_cell))
            {
                currentCell = _cell;
                // Corner-based placement: use cell origin without center offset
                Vector3 pointer = grid.GridToWorldPivot(currentCell);

                if (previewCell != null)
                {
                    previewCell.gameObject.SetActive(true);
                    previewCell.transform.position = pointer;

                    // 배치 가능하면 초록색, 불가능하면 빨간색
                    isPlaceable = CheckCellAvailability(currentCell);
                    previewCell.SetPreviewColor(isPlaceable);
                }
            }
            else
            {
                currentCell = new Int2(-1, -1);
                if (previewCell != null)
                {
                    previewCell.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            currentCell = new Int2(-1, -1);
            if (previewCell != null)
            {
                previewCell.gameObject.SetActive(false);
            }
        }
    }

    private void HandlePlacement()
    {
        if (currentCell.x < 0 || currentCell.z < 0) return;

        // Left click to place
        if (Input.GetMouseButtonDown(0))
        {
            Place(currentCell, placePrefab);
            previewCell.SetPreviewColor(true, true);

            grid.LogCurrentGridState();
            OnPlacementUpdated();
            OnPlacementFinished();
        }
    }

    private void Place(Int2 targetCell, PlaceableObject prefab)
    {
        Vector3 pointer = grid.GridToWorldPivot(targetCell);

        rootIndex.TryGetValue(prefab.Type.PlaceableType, out var parent);
        Transform placed = Instantiate(prefab.transform, pointer, Quaternion.identity, parent);

        SetCellsOccupied(targetCell, true);
    }

    private bool CheckCellAvailability(Int2 rootCell)
    {
        if (grid == null || placePrefab == null) return false;
        return grid.CanOccupyRect(rootCell, placePrefab.CellSize);
    }

    private void SetCellsOccupied(Int2 rootCell, bool value)
    {
        if (grid == null || placePrefab == null) return;
        grid.SetOccupiedRect(rootCell, placePrefab.CellSize, value, placePrefab.Type);
    }

    private void OnPlacementUpdated()
    {
        if (App.GameData == null) return;
        SavePlacementData();
        Debug.Log("[GameSessionRunner] Placement data updated in GameMetaData.");
    }

    private void OnPlacementFinished()
    {
        isPlacing = false;
        if (previewCell != null)
        {
            Destroy(previewCell.gameObject);
            previewCell = null;
        }
    }

    private void SavePlacementData()
    {
        App.SetPlacementData(new PlacementData(grid.GetGridSize(), grid.GetGridRecords()));
    }

    private void CleanPlacementData()
    {
        App.SetPlacementData(new PlacementData(grid.GetGridSize(), new PlacementRecord[grid.GetGridSize().x, grid.GetGridSize().z]));
    }

    private void LoadPlacementData()
    {
        Debug.Log("[PlacementSystem] Loading placement data from GameMetaData...");

        if (!App.HasGameData)
            return;

        PlacementRecord[,] data = App.GetPlacementData().Placements;

        if (data == null)
        {
            Debug.Log("[PlacementSystem] No placement data found.");
            return;
        }

        grid.SetGridRecords(data);

        Dictionary<Int2, PlaceableDTO> roots = new Dictionary<Int2, PlaceableDTO>();

        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int z = 0; z < data.GetLength(1); z++)
            {
                var record = data[x, z];
                if (record.occupied)
                {
                    roots[record.root] = record.placeable;
                }
            }
        }

        foreach (var kvp in roots)
        {
            Int2 root = kvp.Key;
            PlaceableDTO dto = kvp.Value;

            GameObject prefab = null;
            switch (dto.type)
            {
                case PlaceableType.Facility:
                    prefab = registry.GetGameObjectPrefab(dto.facilityType);
                    break;
                case PlaceableType.Tile:
                    prefab = registry.GetGameObjectPrefab(dto.tileType);
                    break;
                case PlaceableType.Decoration:
                    prefab = registry.GetGameObjectPrefab(dto.decorationType);
                    break;
            }

            Place(root, prefab.GetComponent<PlaceableObject>());
        }

    }

    [ContextMenu("DEBUG: Clear Placements")]
    public void ClearPlacementsEditor()
    {
        ClearPlacements();
        CleanPlacementData();
    }

    private void ClearPlacements()
    {
        for (int i = buildingRoots.Count - 1; i >= 0; i--)
        {
            var root = buildingRoots[i].root;
            if (root == null) continue;

            for (int j = root.childCount - 1; j >= 0; j--)
            {
                DestroyImmediate(root.GetChild(j).gameObject);
            }
        }

        // Clear grid occupancy too
        if (grid != null)
        {
            grid.ClearAllOccupancy();
        }

        Debug.Log("[PlacementSystem] Cleared all placements and saved state.");
    }

}

