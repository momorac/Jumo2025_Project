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
        if (!App.SessionService.TryOccupyRandomSeat(out var seat))
            return;

        SpawnCustomer(seat);

    }

    private void SpawnCustomer(Transform seat)
    {
        if (seat == null) return;

        Debug.Log($"<color=green>Spawning customer #{spawnedCustomers + 1} at seat.</color>");

        var customerGO = new GameObject("Customer");
        customerGO.transform.SetParent(seat, worldPositionStays: false);
        customerGO.transform.localPosition = Vector3.zero;
        customerGO.transform.localRotation = Quaternion.identity;
    }
}
