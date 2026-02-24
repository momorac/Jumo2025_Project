using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawnSimSystem : ISimSystem
{
    private CustomerPool pool;

    private int spawnedCustomers = 0;
    private bool hasAvailableSeat;

    public void Initialize()
    {
        pool = App.PoolService.customerPool;

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
            SpawnCustomerDelay(seat);
        }
    }

    private void OnSeatsChanged(Seat seat, bool isAvailable)
    {
        hasAvailableSeat = App.SessionService.GetAvailableSeatsCount() > 0;
    }

    private void SpawnCustomerDelay(Seat seat)
    {
        float delay = Random.Range(3f, 10f);

        Debug.Log($"<color=green>Spawning customer #{spawnedCustomers + 1} at seat {delay} seconds after.</color>");

        Customer instance = pool.Get();
        instance.SetSeatDealy(seat, delay);

        spawnedCustomers++;
    }
}
