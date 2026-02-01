using UnityEngine;

public class PoolService
{
    private PoolRegistry poolRegistry;

    public PedestrianPool pedestrianPool;
    public CustomerPool customerPool;


    public PoolService(PoolRegistry registry)
    {
        poolRegistry = registry;
        pedestrianPool = new PedestrianPool(poolRegistry.pedestrianPoolEntry);
        customerPool = new CustomerPool(poolRegistry.customerPoolEntry);
    }
}
