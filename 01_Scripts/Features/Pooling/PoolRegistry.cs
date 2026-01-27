using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolRegistry", menuName = "Scriptable Objects/PoolRegistry")]
public class PoolRegistry : ScriptableObject
{
    public PoolEntry pedestrianPoolEntry;
}

[Serializable]
public class PoolEntry
{
    public Component prefab;
    public Transform root;

    public bool _collectionCheck;
    public int _defaultCapacity;
    public int _maxSize;

}
