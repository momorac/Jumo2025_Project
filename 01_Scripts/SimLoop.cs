
using System.Collections.Generic;

public class SimLoop
{
    private readonly SimClock simClock;
    private readonly List<ISimSystem> systems = new();

    private float accumulatedTime;
    private bool isEnabled;

    private const float TICK = 0.2f;

    public SimLoop(SimClock simClock)
    {
        this.simClock = simClock;
    }

    public void AddSystem(ISimSystem system)
    {
        systems.Add(system);
    }

    public void SetEnabled(bool value)
    {
        isEnabled = value;
    }

    public void Update(float deltaTime)
    {
        if (!isEnabled) return;

        accumulatedTime += deltaTime;

        while (accumulatedTime >= TICK)
        {
            Tick(TICK);
            accumulatedTime -= TICK;
        }
    }

    private void Tick(float deltaTime)
    {
        simClock.Tick(deltaTime);

        foreach (var system in systems)
        {
            system.Tick(deltaTime);
        }
    }
}
