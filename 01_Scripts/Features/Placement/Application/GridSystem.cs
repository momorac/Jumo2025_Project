using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Int2 size;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 origin;
    public Int2 Size => size;

    private int width => size.x;
    private int height => size.z;

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

    public bool IsInBounds(Int2 cell)
    {
        return IsInBounds(cell.x, cell.z);
    }

    public bool IsOccupied(Int2 cell)
    {
        if (!IsInBounds(cell)) return true; // Treat out-of-bounds as occupied
        return grid[cell.x, cell.z].occupied;
    }

    public void SetOccupied(Int2 root, Int2 cell, bool value)
    {
        if (!IsInBounds(cell)) return;
        grid[cell.x, cell.z].occupied = value;
        grid[cell.x, cell.z].root = root;
    }

    // Occupy or free a rectangle region starting at root (lower-left), with given size (width,height)
    public void SetOccupiedRect(Int2 root, Int2 size, bool value)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dz = 0; dz < size.z; dz++)
            {
                var cell = new Int2(root.x + dx, root.z + dz);
                if (IsInBounds(cell))
                {
                    SetOccupied(root, cell, value);
                }
            }
        }
    }

    // Check if a rectangle can be occupied: in-bounds and all cells are free
    public bool CanOccupyRect(Int2 root, Int2 size)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dz = 0; dz < size.z; dz++)
            {
                var cell = new Int2(root.x + dx, root.z + dz);
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
                grid[x, z].root = new Int2(-1, -1);
            }
        }
    }

    // Convert a world position to grid cell indices
    public Int2 WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize);
        return new Int2(x, z);
    }

    // Return the origin (lower-left) world position of a given grid cell
    public Vector3 GridToWorldPivot(Int2 cell)
    {
        float cx = origin.x + (cell.x) * cellSize;
        float cz = origin.z + (cell.z) * cellSize;
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

    public Int2 GetGridSize()
    {
        return size;
    }

    public PlacementRecord[,] GetGridRecords()
    {
        return grid.Clone() as PlacementRecord[,];
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
