using UnityEngine;
using System;

public class PlacementSystem : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GridSystem grid;

    [Header("Prefabs")]
    [SerializeField] private Placeable placePrefab;

    [Header("Raycast")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float rayMaxDistance = 200f;


    private Placeable previewCell;
    private Vector2Int currentCell = new Vector2Int(-1, -1);
    private bool isPlaceable = true;


    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (placePrefab != null)
        {
            previewCell = Instantiate(placePrefab);
            previewCell.gameObject.SetActive(false);
        }
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
                Transform placed = Instantiate(placePrefab.transform, pointer, Quaternion.identity);
            }

            SetCellsOccupied(currentCell, true);
            previewCell.SetPreviewColor(true, true);
        }
    }


    private bool CheckCellAvailability(Vector2Int rootCell)
    {
        if (grid == null || placePrefab == null) return false;

        Vector2Int size = placePrefab.CellSize;

        for (int width = 0; width < size.x; width++)
        {
            for (int height = 0; height < size.y; height++)
            {
                Vector2Int cell = new Vector2Int(rootCell.x + width, rootCell.y + height);
                if (!grid.IsInBounds(cell) || grid.IsOccupied(cell))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void SetCellsOccupied(Vector2Int rootCell, bool value)
    {
        if (grid == null) return;

        Vector2Int size = placePrefab.CellSize;

        for (int width = 0; width < size.x; width++)
        {
            for (int height = 0; height < size.y; height++)
            {
                Vector2Int cell = new Vector2Int(rootCell.x + width, rootCell.y + height);
                grid.SetOccupied(cell, value);
            }
        }
    }

}

