using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryParentEntry
{
    public IngredientCategory type;
    public Transform rt;
}

public class InventoryView : WindowViewBase
{
    [Header("Prefab References")]
    [SerializeField] private GameObject inventoryItemCellPrefab;

    [Header("UI Elements")]
    [SerializeField] private Button button_close;
    [SerializeField] private List<InventoryParentEntry> contentParents;

    public event Action CloseButtonClicked;

    private readonly Dictionary<IngredientType, InventoryCellView> cellMap = new Dictionary<IngredientType, InventoryCellView>();

    private void Awake()
    {
        button_close.onClick.AddListener(() => CloseButtonClicked?.Invoke());
    }

    public void BuildCell(IngredientType type, string displayName, Sprite icon, int amount)
    {
        var parentEntry = contentParents.Find(entry => entry.type == App.IngredientService.GetCategory(type));
        if (parentEntry == null) return;

        GameObject cellObject = Instantiate(inventoryItemCellPrefab, parentEntry.rt);
        var cellView = cellObject.GetComponent<InventoryCellView>();
        cellView.Bind(type, displayName, icon, amount);
        cellMap[type] = cellView;
    }

    public void UpdateCellAmount(IngredientType type, int amount)
    {
        if (cellMap.TryGetValue(type, out var cell))
            cell.SetAmount(amount);
    }

    public void ClearCells()
    {
        if (contentParents == null) return;
        foreach (var parentEntry in contentParents)
        {
            if (parentEntry.rt == null) continue;
            for (int i = parentEntry.rt.childCount - 1; i >= 0; i--)
                Destroy(parentEntry.rt.GetChild(i).gameObject);
        }
        cellMap.Clear();
    }
}