using UnityEngine;
using UnityEngine.Pool;

public class PedestrianPool : Pool<Pedestrian>
{
    [Header("Pool Settings")]
    [SerializeField] private Pedestrian prefab;
    [SerializeField] private bool _collectionCheck = true;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 100;

    private IObjectPool<Pedestrian> pool;

    public override Pedestrian Prefab => prefab;
    public override bool collectionCheck => _collectionCheck;
    public override int defaultCapacity => _defaultCapacity;
    public override int maxSize => _maxSize;

    private void Awake()
    {
        pool = CreatePool();
    }

    public Pedestrian Get() => pool.Get();
    public void Release(Pedestrian instance) => pool.Release(instance);
}
