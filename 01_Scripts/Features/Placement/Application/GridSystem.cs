using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int size;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 origin;
    public Vector2Int Size => size;

    private int width => size.x;
    private int height => size.y;

    [Header("Debug / Visuals")]
    public bool drawGizmos;
    public Color gridColor;
    public Color boundsColor;

    private PlacementRecord[,] grid;

    private void Awake()
    {
        grid = new PlacementRecord[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid[x, z] = new PlacementRecord();
            }
        }
    }

    private void Start()
    {
        App.SetPlacementData(new PlacementData(size, grid));
    }

    public bool IsInBounds(int x, int z)
    {
        return x >= 0 && z >= 0 && x < width && z < height;
    }

    public bool IsInBounds(Vector2Int cell)
    {
        return IsInBounds(cell.x, cell.y);
    }

    public bool IsOccupied(Vector2Int cell)
    {
        if (!IsInBounds(cell)) return true; // Treat out-of-bounds as occupied
        return grid[cell.x, cell.y].occupied;
    }

    public void SetOccupied(Vector2Int root, Vector2Int cell, bool value)
    {
        if (!IsInBounds(cell)) return;
        grid[cell.x, cell.y].occupied = value;
        grid[cell.x, cell.y].root = root;
    }

    // Occupy or free a rectangle region starting at root (lower-left), with given size (width,height)
    public void SetOccupiedRect(Vector2Int root, Vector2Int size, bool value)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dz = 0; dz < size.y; dz++)
            {
                var cell = new Vector2Int(root.x + dx, root.y + dz);
                if (IsInBounds(cell))
                {
                    SetOccupied(root, cell, value);
                }
            }
        }
    }

    // Check if a rectangle can be occupied: in-bounds and all cells are free
    public bool CanOccupyRect(Vector2Int root, Vector2Int size)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dz = 0; dz < size.y; dz++)
            {
                var cell = new Vector2Int(root.x + dx, root.y + dz);
                if (!IsInBounds(cell) || IsOccupied(cell))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Clear all occupancy flags
    public void ClearAllOccupancy()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid[x, z].occupied = false;
                grid[x, z].root = new Vector2Int(-1, -1);
            }
        }
    }

    // Convert a world position to grid cell indices
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize);
        return new Vector2Int(x, z);
    }

    // Return the origin (lower-left) world position of a given grid cell
    public Vector3 GridToWorldPivot(Vector2Int cell)
    {
        float cx = origin.x + (cell.x) * cellSize;
        float cz = origin.z + (cell.y) * cellSize;
        return new Vector3(cx, origin.y, cz);
    }

    public void LogCurrentGridState()
    {
        string gridState = "Grid Occupancy:\n";
        for (int z = height - 1; z >= 0; z--)
        {
            for (int x = 0; x < width; x++)
            {
                gridState += grid[x, z].occupied ? "[X]" : "[ ]";
            }
            gridState += "\n";
        }
        Debug.Log(gridState);
    }

    public Vector2Int GetGridSize()
    {
        return size;
    }

    public PlacementRecord[,] GetGridRecords()
    {
        return grid;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = gridColor;

        // Draw grid lines
        Vector3 start = origin;
        Vector3 right = new Vector3(cellSize * width, 0f, 0f);
        Vector3 forward = new Vector3(0f, 0f, cellSize * height);

        // Lines along Z
        for (int x = 0; x <= width; x++)
        {
            Vector3 a = start + new Vector3(x * cellSize, 0f, 0f);
            Vector3 b = a + forward;
            Gizmos.DrawLine(a, b);
        }

        // Lines along X
        for (int z = 0; z <= height; z++)
        {
            Vector3 a = start + new Vector3(0f, 0f, z * cellSize);
            Vector3 b = a + right;
            Gizmos.DrawLine(a, b);
        }

        // Draw bounds rectangle
        Gizmos.color = boundsColor;
        Vector3 p0 = origin;
        Vector3 p1 = origin + right;
        Vector3 p2 = origin + right + forward;
        Vector3 p3 = origin + forward;
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);
    }
}
