using UnityEngine;

public class PedestrianSpawnSimSystem : ISimSystem
{
    private PedestrianPool pool;
    private float minSpawnInterval = 1f;
    private float maxSpawnInterval = 5f;
    private float timeUntilNextSpawn;

    public void Initialize()
    {
        pool = App.PoolService.pedestrianPool;
        ResetNextSpawnTimer();
    }

    public void Tick(float deltaTime)
    {
        if (pool == null)
        {
            Debug.LogWarning("PedestrianSpawnSimSystem: Pool is null.");
            return;
        }

        timeUntilNextSpawn -= deltaTime;
        if (timeUntilNextSpawn <= 0f)
        {
            SpawnPedestrian();
            ResetNextSpawnTimer();
        }
    }

    private void ResetNextSpawnTimer()
    {
        // Randomize next interval between [minSpawnInterval, maxSpawnInterval]
        float min = Mathf.Min(minSpawnInterval, maxSpawnInterval);
        float max = Mathf.Max(minSpawnInterval, maxSpawnInterval);
        timeUntilNextSpawn = Random.Range(min, max);
    }

    private void SpawnPedestrian()
    {
        Pedestrian instance = pool.Get();
        if (instance == null)
        {
            Debug.LogWarning("PedestrianPool returned null instance.");
            return;
        }
        else
        {
            instance.OnSpawn();
        }
    }
}
