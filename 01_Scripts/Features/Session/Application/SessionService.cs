using System;
using System.Collections.Generic;
using UnityEngine;

public class SessionService
{
    private readonly SessionState sessionState;

    public SessionService(SessionState sessionState)
    {
        this.sessionState = sessionState ?? throw new ArgumentNullException(nameof(sessionState));
        sessionState.Seats = new Dictionary<Transform, bool>();
    }

    public event Action<Transform, bool> OnSeatsChanged;

    public void RegisterSeat(Transform seat)
    {
        sessionState.Seats[seat] = true;
        sessionState.AvailableSeatsCount++;
        OnSeatsChanged?.Invoke(seat, true);

        Debug.Log("<color=green>Seat registered. Total available seats: " + sessionState.AvailableSeatsCount + "</color>");
    }

    public bool TryOccupyRandomSeat(out Transform seat)
    {
        seat = null;

        if (sessionState == null || sessionState.Seats == null || sessionState.AvailableSeatsCount <= 0)
            return false;

        // 가용 좌석 목록 수집
        List<Transform> availableSeats = new List<Transform>();
        foreach (var kvp in sessionState.Seats)
        {
            if (kvp.Value)
            {
                availableSeats.Add(kvp.Key);
            }
        }

        if (availableSeats.Count == 0)
            return false;

        int randomIndex = UnityEngine.Random.Range(0, availableSeats.Count);
        seat = availableSeats[randomIndex];
        if (seat == null)
            return false;

        sessionState.Seats[seat] = false;
        sessionState.AvailableSeatsCount--;
        OnSeatsChanged?.Invoke(seat, false);
        return true;
    }

    public int GetAvailableSeatsCount()
    {
        if (sessionState == null)
            return 0;

        return sessionState.AvailableSeatsCount;
    }
}
