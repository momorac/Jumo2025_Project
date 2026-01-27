using UnityEngine;
using UnityEngine.Pool;

public abstract class Pool<T> : MonoBehaviour where T : MonoBehaviour
{
    public abstract T Prefab { get; }

    public abstract bool collectionCheck { get; }
    public abstract int defaultCapacity { get; }
    public abstract int maxSize { get; }

    protected IObjectPool<T> CreatePool()
    {
        return new ObjectPool<T>(
            OnCreateObject,
            OnGetFromPool,
            OnReleaseToPool,
            OnDestroyPooledObject,
            collectionCheck,
            defaultCapacity,
            maxSize
        );
    }

    private T OnCreateObject()
    {
        T instance = Instantiate(Prefab);
        return instance;
    }

    private void OnGetFromPool(T obj)
    {
        obj.gameObject.SetActive(true);
    }

    private void OnReleaseToPool(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void OnDestroyPooledObject(T obj)
    {
        Destroy(obj.gameObject);
    }

}
