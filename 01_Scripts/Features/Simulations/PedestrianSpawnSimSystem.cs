using UnityEngine;

public class PedestrianSpawnSimSystem : ISimSystem
{
    private PedestrianPool pool;
    private float minSpawnInterval = 8f;
    private float maxSpawnInterval = 20f;
    private float timeUntilNextSpawn;

    public void Initialize()
    {
        pool = App.PoolService.pedestrianPool;

        if (pool == null)
        {
            Debug.LogWarning("PedestrianSpawnSimSystem: Pool is null.");
            return;
        }

        ResetNextSpawnTimer();
    }

    public void Tick(float deltaTime)
    {
        // Debug.Log($"Time until next pedestrian spawn: {timeUntilNextSpawn:F2} seconds.");

        timeUntilNextSpawn -= deltaTime;
        if (timeUntilNextSpawn <= 0f)
        {
            SpawnPedestrian();
            ResetNextSpawnTimer();
        }
    }

    private void SpawnPedestrian()
    {
        Pedestrian instance = pool.Get();
        instance.OnWalkComplete += () => pool.Release(instance);

        if (instance == null)
        {
            Debug.LogWarning("PedestrianPool returned null instance.");
            return;
        }
    }

    private void ResetNextSpawnTimer()
    {
        timeUntilNextSpawn = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

}
