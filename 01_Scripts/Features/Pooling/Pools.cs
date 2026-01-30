using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class PedestrianPool : Pool<Pedestrian>
{
    private IObjectPool<Pedestrian> pool;
    private PoolEntry poolEntry;

    public override Pedestrian Prefab => poolEntry.prefab as Pedestrian;
    public override Transform Root => App.Anchors.PedestrianRoot;

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

public class CustomerPool : Pool<Customer>
{
    private IObjectPool<Customer> pool;
    private PoolEntry poolEntry;

    public override Customer Prefab => poolEntry.prefab as Customer;
    public override Transform Root => App.Anchors.CustomerRoot;

    public override bool collectionCheck => poolEntry._collectionCheck;
    public override int defaultCapacity => poolEntry._defaultCapacity;
    public override int maxSize => poolEntry._maxSize;

    public CustomerPool(PoolEntry customerPoolEntry)
    {
        poolEntry = customerPoolEntry;
        pool = CreatePool();
    }

    public Customer Get() => pool.Get();
    public void Release(Customer instance) => pool.Release(instance);
}