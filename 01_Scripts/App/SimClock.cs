using UnityEngine;

public class SimClock
{
    public int Day { get; private set; }
    public float Time01 { get; private set; }
    public float DayLengthSeconds { get; }

    public SimClock(float dayLengthSeconds)
    {
        DayLengthSeconds = dayLengthSeconds;
    }

    public void Tick(float deltaSeconds)
    {
        Time01 += deltaSeconds / DayLengthSeconds;
    }

    public bool IsDayOver()
    {
        return Time01 >= 1f;
    }

    public void NextDay()
    {
        Day++;
        Time01 = 0f;
    }
}
