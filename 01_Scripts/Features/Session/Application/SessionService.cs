using System;
using System.Collections.Generic;

public class SessionService
{
    private readonly SessionMeta sessionMeta;

    public SessionService(SessionMeta sessionMeta)
    {
        this.sessionMeta = sessionMeta ?? throw new ArgumentNullException(nameof(sessionMeta));
        sessionMeta.Seats = new Dictionary<Seat, bool>();
    }

    public event Action<Seat, bool> OnSeatsChanged;

    public void RegisterSeat(Seat seat)
    {
        sessionMeta.Seats[seat] = true;
        sessionMeta.AvailableSeatsCount++;
        OnSeatsChanged?.Invoke(seat, true);

        GameLogger.LogVerbose(LogCategory.System, $"Seat registered. Available: {sessionMeta.AvailableSeatsCount}");
    }

    public bool TryOccupyRandomSeat(out Seat seat)
    {
        seat = null;

        if (sessionMeta == null || sessionMeta.Seats == null || sessionMeta.AvailableSeatsCount <= 0)
            return false;

        // 가용 좌석 목록 수집
        List<Seat> availableSeats = new List<Seat>();
        foreach (var kvp in sessionMeta.Seats)
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

        sessionMeta.Seats[seat] = false;
        sessionMeta.AvailableSeatsCount--;
        OnSeatsChanged?.Invoke(seat, false);
        return true;
    }

    public int GetAvailableSeatsCount()
    {
        if (sessionMeta == null)
            return 0;

        return sessionMeta.AvailableSeatsCount;
    }
}
