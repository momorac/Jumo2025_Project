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
        if (!IsInBounds(cell)) return true; // 범위를 벗어난 경우도 이미 점유된 것으로 취급
        return grid[cell.x, cell.z].occupied;
    }

    public void SetOccupied(Int2 root, Int2 cell, bool value, Placeable _placeable)
    {
        if (!IsInBounds(cell)) return;
        PlacementRecord record = grid[cell.x, cell.z];
        record.occupied = value;
        record.root = root;

        record.placeable.type = _placeable.PlaceableType;
        switch (record.placeable.type)
        {
            case PlaceableType.Facility:
                record.placeable.facility = (_placeable as Facility).Type;
                break;
            case PlaceableType.Tile:
                record.placeable.tile = (_placeable as Tile).Type;
                break;
            case PlaceableType.Decoration:
                record.placeable.decoration = (_placeable as Decoration).Type;
                break;
        }
    }

    // root에서 시작하는 직사각형 영역을 주어진 크기(가로, 세로)로 점유하거나 비우기
    public void SetOccupiedRect(Int2 root, Int2 size, bool value, Placeable placedType)
    {
        for (int dx = 0; dx < size.x; dx++)
        {
            for (int dz = 0; dz < size.z; dz++)
            {
                var cell = new Int2(root.x + dx, root.z + dz);
                if (IsInBounds(cell))
                {
                    SetOccupied(root, cell, value, placedType);
                }
            }
        }
    }

    // 직사각형 영역을 점유할 수 있는지 확인 (범위 안이고 모든 셀이 비어 있는지)
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

    // 모든 셀의 점유 상태를 초기화
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

    // 월드 좌표를 그리드 셀 인덱스로 변환
    public Int2 WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - origin.z) / cellSize);
        return new Int2(x, z);
    }

    // 주어진 그리드 셀의 원점(왼쪽 아래) 월드 좌표 반환
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

    public void SetGridRecords(PlacementRecord[,] data)
    {
        if (data.GetLength(0) != width || data.GetLength(1) != height)
        {
            Debug.LogError("SetGridRecords: Data size does not match grid size.");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                grid[x, z] = data[x, z];
            }
        }
    }

    public PlacementRecord[,] GetGridRecords()
    {
        return grid.Clone() as PlacementRecord[,];
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        Gizmos.color = gridColor;

        // 그리드 선 그리기
        Vector3 start = origin;
        Vector3 right = new Vector3(cellSize * width, 0f, 0f);
        Vector3 forward = new Vector3(0f, 0f, cellSize * height);

        // Z축 방향 선들
        for (int x = 0; x <= width; x++)
        {
            Vector3 a = start + new Vector3(x * cellSize, 0f, 0f);
            Vector3 b = a + forward;
            Gizmos.DrawLine(a, b);
        }

        // X축 방향 선들
        for (int z = 0; z <= height; z++)
        {
            Vector3 a = start + new Vector3(0f, 0f, z * cellSize);
            Vector3 b = a + right;
            Gizmos.DrawLine(a, b);
        }

        // 그리드 경계 사각형 그리기
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
