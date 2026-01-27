using System.Collections.Generic;
using UnityEngine;

public class CustomerSpawnSimSystem : ISimSystem
{
    private int spawnedCustomers = 0;
    private List<Table> trackedTables = new List<Table>();

    public void Initialize()
    {
        // seed with any existing tables
        if (App.SessionData?.tables != null)
        {
            for (int i = 0; i < App.SessionData.tables.Count; i++)
            {
                var t = App.SessionData.tables[i];
                if (t != null && !trackedTables.Contains(t)) trackedTables.Add(t);
            }
        }

        // subscribe to updates for newly placed tables
        App.SessionData.OnTableChanged += (Table table) =>
        {
            if (table != null && !trackedTables.Contains(table))
            {
                trackedTables.Add(table);
            }
        };
    }

    public void Tick(float deltaTime)
    {
        if (trackedTables == null || trackedTables.Count == 0)
        {
            return;
        }

        bool allOccupied = true;
        Table candidate = null;

        for (int i = 0; i < trackedTables.Count; i++)
        {
            var table = trackedTables[i];
            if (table == null) continue;
            if (!table.IsFullyOccupied())
            {
                allOccupied = false;
                if (table.HasAvailableSeat())
                {
                    candidate = table;
                    break;
                }
            }
        }

        if (allOccupied || candidate == null)
        {
            return;
        }

        SpawnCustomer(candidate);
        spawnedCustomers++;

    }

    private void SpawnCustomer(Table table)
    {
        if (table == null) return;

        Debug.Log($"<color=green>Spawning customer #{spawnedCustomers + 1} at table.</color>");
        var seat = table.GetFirstAvailableSeat();
        if (seat == null) return;

        var customerGO = new GameObject("Customer");
        customerGO.transform.SetParent(seat, worldPositionStays: false);
        customerGO.transform.localPosition = Vector3.zero;
        customerGO.transform.localRotation = Quaternion.identity;
    }
}
