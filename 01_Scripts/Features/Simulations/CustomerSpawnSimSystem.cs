using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawnSimSystem : ISimSystem
{
    private int spawnedCustomers = 0;


    public void Initialize()
    {
    }

    public void Tick(float deltaTime)
    {
        if (App.SessionState?.Seats == null || App.SessionState.AvailableSeatsCount == 0)
            return;

        Transform seat = App.SessionState.GetAvailableRandomSeat();
        SpawnCustomer(seat);

    }

    private void SpawnCustomer(Transform seat)
    {
        if (seat == null) return;

        Debug.Log($"<color=green>Spawning customer #{spawnedCustomers + 1} at seat.</color>");

        App.SessionState.Seats[seat] = false;
        App.SessionState.AvailableSeatsCount--;

        var customerGO = new GameObject("Customer");
        customerGO.transform.SetParent(seat, worldPositionStays: false);
        customerGO.transform.localPosition = Vector3.zero;
        customerGO.transform.localRotation = Quaternion.identity;
    }
}
