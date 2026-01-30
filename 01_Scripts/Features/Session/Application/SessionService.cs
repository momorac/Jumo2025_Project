using System;
using UnityEngine;

public class SessionService
{
    private readonly SessionState sessionState;

    public SessionService(SessionState sessionState)
    {
        this.sessionState = sessionState ?? throw new ArgumentNullException(nameof(sessionState));
    }

    public event Action<Transform, bool> OnSeatsChanged
    {
        add
        {
            if (sessionState != null)
            {
                sessionState.OnSeatsChanged += value;
            }
        }
        remove
        {
            if (sessionState != null)
            {
                sessionState.OnSeatsChanged -= value;
            }
        }
    }

    public void RegisterSeat(Transform seat)
    {
        if (seat == null) return;
        sessionState.RegisterSeat(seat);
    }

    public bool TryOccupyRandomSeat(out Transform seat)
    {
        seat = null;

        if (sessionState == null || sessionState.Seats == null || sessionState.AvailableSeatsCount <= 0)
            return false;

        seat = sessionState.GetAvailableRandomSeat();
        if (seat == null)
            return false;

        sessionState.Seats[seat] = false;
        sessionState.AvailableSeatsCount--;
        return true;
    }

    public int GetAvailableSeatsCount()
    {
        if (sessionState == null)
            return 0;

        return sessionState.AvailableSeatsCount;
    }
}
