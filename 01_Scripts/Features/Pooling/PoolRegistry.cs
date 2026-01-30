using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolRegistry", menuName = "Scriptable Objects/PoolRegistry")]
public class PoolRegistry : ScriptableObject
{
    public PoolEntry pedestrianPoolEntry;
    public PoolEntry customerPoolEntry;
}

[Serializable]
public class PoolEntry
{
    public bool _collectionCheck;
    public int _defaultCapacity;
    public int _maxSize;

    public Component prefab;
}
