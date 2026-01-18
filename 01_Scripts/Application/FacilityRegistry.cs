using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Facility Registry", fileName = "FacilityRegistry")]
public class FacilityRegistry : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public FacilityType type;
        public GameObject prefab;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();

    public GameObject GetPrefab(FacilityType type)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].type == type) return entries[i].prefab;
        }
        return null;
    }

    public IReadOnlyList<Entry> Entries => entries;
}
