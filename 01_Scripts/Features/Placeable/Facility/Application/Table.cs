using UnityEngine;

public class Table : MonoBehaviour, IFacilityService
{
    public Placeable Type { get; private set; }

    [SerializeField] private Transform[] seatRoot;
    public int Capacity => seatRoot.Length;

    public bool IsFullyOccupied()
    {
        if (seatRoot == null || seatRoot.Length == 0) return true;
        for (int i = 0; i < seatRoot.Length; i++)
        {
            if (seatRoot[i] != null && seatRoot[i].childCount == 0)
            {
                return false;
            }
        }
        return true;
    }

    public bool HasAvailableSeat()
    {
        if (seatRoot == null || seatRoot.Length == 0) return false;
        for (int i = 0; i < seatRoot.Length; i++)
        {
            if (seatRoot[i] != null && seatRoot[i].childCount == 0)
            {
                return true;
            }
        }
        return false;
    }

    public Transform GetFirstAvailableSeat()
    {
        if (seatRoot == null) return null;
        for (int i = 0; i < seatRoot.Length; i++)
        {
            Transform seat = seatRoot[i];
            if (seat != null && seat.childCount == 0)
            {
                return seat;
            }
        }
        return null;
    }

    public void Initialize(Placeable _placeable)
    {
        this.Type = _placeable;
    }

    public void OnPlaced()
    {
        App.SessionData.RegisterTable(this);

        Debug.Log($"<color=green>Table placed. Capacity: {Capacity}. Total available seats: {App.SessionData.tables}</color>");
    }

    public void OnRemoved()
    {
    }

}
