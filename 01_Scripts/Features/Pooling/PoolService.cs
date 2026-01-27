using UnityEngine;

public class PoolService
{
    private PoolRegistry poolRegistry;

    public PedestrianPool pedestrianPool;


    public PoolService()
    {
        poolRegistry = Resources.Load<PoolRegistry>("RegistrySettings/PoolRegistry");

        pedestrianPool = new PedestrianPool(poolRegistry.pedestrianPoolEntry);
    }
}
