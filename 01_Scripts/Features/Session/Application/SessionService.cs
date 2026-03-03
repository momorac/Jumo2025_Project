using System;
using System.Collections.Generic;

public class SessionService
{
    private readonly SessionState sessionState;

    public SessionService(SessionState sessionState)
    {
        this.sessionState = sessionState ?? throw new ArgumentNullException(nameof(sessionState));
        sessionState.Seats = new Dictionary<Seat, bool>();
    }

    public event Action<Seat, bool> OnSeatsChanged;

    public void RegisterSeat(Seat seat)
    {
        sessionState.Seats[seat] = true;
        sessionState.AvailableSeatsCount++;
        OnSeatsChanged?.Invoke(seat, true);

        GameLogger.LogVerbose(LogCategory.System, $"Seat registered. Available: {sessionState.AvailableSeatsCount}");
    }

    public bool TryOccupyRandomSeat(out Seat seat)
    {
        seat = null;

        if (sessionState == null || sessionState.Seats == null || sessionState.AvailableSeatsCount <= 0)
            return false;

        // 가용 좌석 목록 수집
        List<Seat> availableSeats = new List<Seat>();
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
