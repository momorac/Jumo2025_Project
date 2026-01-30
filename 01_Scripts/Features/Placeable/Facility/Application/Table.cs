using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IFacilityService
{
    public Placeable Type { get; private set; }

    [SerializeField] private Transform[] seatRoot;
    public int Capacity => seatRoot.Length;

    public void Initialize(Placeable _placeable)
    {
        this.Type = _placeable;
        for (int i = 0; i < seatRoot.Length; i++)
        {
            App.SessionService.RegisterSeat(seatRoot[i]);
        }
    }

    public void OnPlaced()
    {
        Debug.Log($"<color=green>Table placed. Capacity: {Capacity}.</color>");
    }

    public void OnRemoved()
    {
    }

}
