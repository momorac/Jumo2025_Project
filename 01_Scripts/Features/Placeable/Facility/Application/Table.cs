using UnityEngine;

public class Table : MonoBehaviour, IFacilityService
{
    public Placeable Type { get; private set; }

    [SerializeField] private Transform[] seatRoot;
    public int Capacity => seatRoot.Length;

    public void Initialize(Placeable _placeable)
    {
        this.Type = _placeable;
    }

    public void OnPlaced()
    {
        App.SessionData.ModifySeats(Capacity);

        Debug.Log($"<color=green>Table placed. Capacity: {Capacity}. Total available seats: {App.SessionData.availableSeats}</color>");
    }

    public void OnRemoved()
    {
    }

}
