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


    private void Start()
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
    }

    private void Update()
    {
        if (!isPlacing)
            return;

        UpdatePreview();
        if (isPlaceable)
        {
            HandlePlacement();
        }
    }

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
            // Corner-based placement: use cell origin without center offset
            Vector3 pointer = grid.GridToWorldPivot(currentCell);
            if (placePrefab != null)
            {
                rootIndex.TryGetValue(placePrefab.Type.PlacementType, out var parent);
                Transform placed = Instantiate(placePrefab.transform, pointer, Quaternion.identity, parent);
            }

            SetCellsOccupied(currentCell, true);
            previewCell.SetPreviewColor(true, true);

            grid.LogCurrentGridState();
            OnPlacementUpdated();
        }
    }

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

    private bool CheckCellAvailability(Int2 rootCell)
    {
        if (grid == null || placePrefab == null) return false;
        return grid.CanOccupyRect(rootCell, placePrefab.CellSize);
    }

    private void SetCellsOccupied(Int2 rootCell, bool value)
    {
        if (grid == null || placePrefab == null) return;
        grid.SetOccupiedRect(rootCell, placePrefab.CellSize, value);
    }

    private void OnPlacementUpdated()
    {
        if (App.GameData == null) return;
        App.SetPlacementData(new PlacementData(grid.GetGridSize(), grid.GetGridRecords()));
        Debug.Log("[GameSessionRunner] Placement data updated in GameMetaData.");
    }

    public Int2 GetGridSize()
    {
        return grid.GetGridSize();
    }


    // private void LoadPlacements()
    // {
    //     isLoading = true;
    //     try
    //     {
    //         var data = PlacementSaveService.Load();
    //         if (data == null || data.records == null) return;

    //         foreach (var rec in data.records)
    //         {
    //             var cell = new Vector2Int(rec.x, rec.y);
    //             if (!grid.IsInBounds(cell)) continue;

    //             Placeable prefabToUse = null;
    //             // Prefer ID-based lookup
    //             if (rec.id != 0 && prefabIndexById.TryGetValue(rec.id, out var foundById))
    //             {
    //                 prefabToUse = foundById;
    //             }
    //             else if (placePrefab != null)
    //             {
    //                 prefabToUse = placePrefab; // fallback
    //             }

    //             if (prefabToUse == null) continue;

    //             Vector3 pos = grid.GridToWorldPivot(cell);
    //             rootIndex.TryGetValue(prefabToUse.Type, out var parent);
    //             Instantiate(prefabToUse.transform, pos, Quaternion.identity, parent);

    //             // Occupy cells based on prefab size
    //             grid.SetOccupiedRect(cell, prefabToUse.CellSize, true);

    //             placed.Add(new PlacementRecord { id = ComputeStableId(prefabToUse.name), x = cell.x, y = cell.y });
    //         }
    //     }
    //     finally
    //     {
    //         isLoading = false;
    //     }
    // }

    [ContextMenu("DEBUG: Clear Placements")]
    public void ClearPlacementsEditor()
    {
        ClearPlacements();
    }

    public void ClearPlacements()
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

