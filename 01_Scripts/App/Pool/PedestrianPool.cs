using UnityEngine;
using UnityEngine.Pool;

public class PedestrianPool : Pool<Pedestrian>
{
    private IObjectPool<Pedestrian> pool;
    private PoolEntry poolEntry;

    public override Pedestrian Prefab => poolEntry.prefab as Pedestrian;
    public override bool collectionCheck => poolEntry._collectionCheck;
    public override int defaultCapacity => poolEntry._defaultCapacity;
    public override int maxSize => poolEntry._maxSize;

    public PedestrianPool(PoolEntry pedestrianPoolEntry)
    {
        poolEntry = pedestrianPoolEntry;
        pool = CreatePool();
    }

    public Pedestrian Get() => pool.Get();
    public void Release(Pedestrian instance) => pool.Release(instance);
}
