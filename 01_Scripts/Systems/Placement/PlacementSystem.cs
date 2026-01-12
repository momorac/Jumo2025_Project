using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BuildingRoot
{
    public PlaceableType type;
    public Transform root;
}

public class PlacementSystem : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GridSystem grid;

    [Header("Prefabs")]
    [SerializeField] private Placeable placePrefab;
    [SerializeField] private List<Placeable> placeablePrefabs = new List<Placeable>();

    [Header("Parents by Type")]
    [SerializeField] private List<BuildingRoot> buildingRoots = new List<BuildingRoot>();

    [Header("Raycast")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rayMaxDistance = 200f;


    private Placeable previewCell;
    private Vector2Int currentCell = new Vector2Int(-1, -1);
    private bool isPlaceable = true;

    // Save/Load state
    private readonly List<PlacementRecord> placed = new List<PlacementRecord>();
    private readonly Dictionary<string, Placeable> prefabIndex = new Dictionary<string, Placeable>();
    private readonly Dictionary<int, Placeable> prefabIndexById = new Dictionary<int, Placeable>();
    private readonly Dictionary<PlaceableType, Transform> rootIndex = new Dictionary<PlaceableType, Transform>();
    private bool isLoading;


    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Build prefab index by name for load-time lookup
        if (placePrefab != null && !placeablePrefabs.Contains(placePrefab))
            placeablePrefabs.Add(placePrefab);

        foreach (var p in placeablePrefabs)
        {
            if (p == null) continue;
            if (!prefabIndex.ContainsKey(p.name))
                prefabIndex[p.name] = p;
            var id = p.StableId != 0 ? p.StableId : ComputeStableId(p.name);
            if (!prefabIndexById.ContainsKey(id))
                prefabIndexById[id] = p;
        }

        // Map building type -> parent root
        rootIndex.Clear();
        foreach (var br in buildingRoots)
        {
            if (br != null && br.root != null)
            {
                rootIndex[br.type] = br.root;
            }
        }

        if (placePrefab != null)
        {
            previewCell = Instantiate(placePrefab);
            previewCell.gameObject.SetActive(false);
        }

        // Load saved placements at startup
        LoadPlacements();
    }

    private void Update()
    {
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
            Vector2Int _cell = grid.WorldToGrid(worldPos);

            if (_cell == currentCell)
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
                    // Optional: align to ground normal
                    // previewInstance.up = hit.normal;

                    // Change color based on multi-cell availability
                    isPlaceable = CheckCellAvailability(currentCell);
                    previewCell.SetPreviewColor(isPlaceable);
                }
            }
            else
            {
                currentCell = new Vector2Int(-1, -1);
                if (previewCell != null)
                {
                    previewCell.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            currentCell = new Vector2Int(-1, -1);
            if (previewCell != null)
            {
                previewCell.gameObject.SetActive(false);
            }
        }
    }

    private void HandlePlacement()
    {
        if (currentCell.x < 0 || currentCell.y < 0) return;

        // Left click to place
        if (Input.GetMouseButtonDown(0))
        {
            // Corner-based placement: use cell origin without center offset
            Vector3 pointer = grid.GridToWorldPivot(currentCell);
            if (placePrefab != null)
            {
                rootIndex.TryGetValue(placePrefab.Type, out var parent);
                Transform placed = Instantiate(placePrefab.transform, pointer, Quaternion.identity, parent);
            }

            SetCellsOccupied(currentCell, true);
            previewCell.SetPreviewColor(true, true);

            grid.LogCurrentGridState();

            // Save record and persist
            var rec = new PlacementRecord
            {
                id = placePrefab != null ? (placePrefab.StableId != 0 ? placePrefab.StableId : ComputeStableId(placePrefab.name)) : 0,
                x = currentCell.x,
                y = currentCell.y
            };
            placed.Add(rec);
            SavePlacements();
        }
    }


    private bool CheckCellAvailability(Vector2Int rootCell)
    {
        if (grid == null || placePrefab == null) return false;
        return grid.CanOccupyRect(rootCell, placePrefab.CellSize);
    }

    private void SetCellsOccupied(Vector2Int rootCell, bool value)
    {
        if (grid == null || placePrefab == null) return;
        grid.SetOccupiedRect(rootCell, placePrefab.CellSize, value);
    }

    private void LoadPlacements()
    {
        isLoading = true;
        try
        {
            var data = PlacementSaveService.Load();
            if (data == null || data.records == null) return;

            foreach (var rec in data.records)
            {
                var cell = new Vector2Int(rec.x, rec.y);
                if (!grid.IsInBounds(cell)) continue;

                Placeable prefabToUse = null;
                // Prefer ID-based lookup
                if (rec.id != 0 && prefabIndexById.TryGetValue(rec.id, out var foundById))
                {
                    prefabToUse = foundById;
                }
                else if (placePrefab != null)
                {
                    prefabToUse = placePrefab; // fallback
                }

                if (prefabToUse == null) continue;

                Vector3 pos = grid.GridToWorldPivot(cell);
                rootIndex.TryGetValue(prefabToUse.Type, out var parent);
                Instantiate(prefabToUse.transform, pos, Quaternion.identity, parent);

                // Occupy cells based on prefab size
                grid.SetOccupiedRect(cell, prefabToUse.CellSize, true);

                placed.Add(new PlacementRecord { id = ComputeStableId(prefabToUse.name), x = cell.x, y = cell.y });
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private void SavePlacements()
    {
        if (isLoading) return;
        var data = new PlacementSaveData { records = new List<PlacementRecord>(placed) };
        PlacementSaveService.Save(data);
    }

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

        placed.Clear();

        PlacementSaveService.Save(new PlacementSaveData());

        Debug.Log("[PlacementSystem] Cleared all placements and saved state.");
    }

    // Deterministic 32-bit FNV-1a hash for stable prefab ID
    private static int ComputeStableId(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        unchecked
        {
            const uint fnvOffset = 2166136261;
            const uint fnvPrime = 16777619;
            uint hash = fnvOffset;
            for (int i = 0; i < s.Length; i++)
            {
                hash ^= s[i];
                hash *= fnvPrime;
            }
            return (int)hash;
        }
    }
}

