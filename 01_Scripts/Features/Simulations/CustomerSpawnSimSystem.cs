using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawnSimSystem : ISimSystem
{
    private int spawnedCustomers = 0;
    private bool hasAvailableSeat;

    public void Initialize()
    {
        hasAvailableSeat = App.SessionService.GetAvailableSeatsCount() > 0;
        App.SessionService.OnSeatsChanged += OnSeatsChanged;
    }

    public void Tick(float deltaTime)
    {
        // 좌석 상태에 변화가 없고, 사용할 수 있는 좌석이 없다면 바로 반환
        if (!hasAvailableSeat)
            return;

        if (App.SessionService.TryOccupyRandomSeat(out var seat))
        {
            SpawnCustomer(seat);
        }
    }

    private void OnSeatsChanged(Transform seat, bool isAvailable)
    {
        hasAvailableSeat = App.SessionService.GetAvailableSeatsCount() > 0;
    }

    private void SpawnCustomer(Transform seat)
    {
        if (seat == null) return;

        Debug.Log($"<color=green>Spawning customer #{spawnedCustomers + 1} at seat.</color>");
        spawnedCustomers++;

        var customerGO = new GameObject("Customer");
        customerGO.transform.SetParent(seat, worldPositionStays: false);
        customerGO.transform.localPosition = Vector3.zero;
        customerGO.transform.localRotation = Quaternion.identity;
    }
}
