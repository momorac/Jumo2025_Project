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

/// <summary>
/// 인벤토리 뷰 스크립트. 인벤토리 셀을 관리하고, UI 이벤트를 처리합니다.
/// </summary>

public class InventoryView : WindowViewBase
{
    [Header("Prefab References")]
    [SerializeField] private InventoryCellView inventoryCellPrefab;

    [Header("UI Elements")]
    [SerializeField] private Button button_close;
    [SerializeField] private List<InventoryParentEntry> contentParents;

    public event Action CloseButtonClicked;

    private readonly Dictionary<IngredientType, InventoryCellView> cellMap = new Dictionary<IngredientType, InventoryCellView>();

    private void Awake()
    {
        button_close.onClick.AddListener(() => CloseButtonClicked?.Invoke());
    }

    /// <summary>> 인벤토리 셀을 생성하여 부모 컨테이너에 추가한다. 셀은 IngredientType으로 식별되며, 셀 뷰는 cellMap에 저장된다. </summary>
    public void BuildCell(IngredientType type, string displayName, Sprite icon, int amount)
    {
        var parentEntry = contentParents.Find(entry => entry.type == App.IngredientService.GetCategory(type));
        if (parentEntry == null) return;

        InventoryCellView cellView = Instantiate(inventoryCellPrefab, parentEntry.rt);
        cellView.Bind(type, displayName, icon, amount);
        cellMap[type] = cellView;
    }

    /// <summary>특정 재료 유형의 셀 뷰가 존재하면 해당 셀의 수량을 업데이트한다.</summary>
    public void UpdateCellAmount(IngredientType type, int amount)
    {
        if (cellMap.TryGetValue(type, out var cell))
            cell.SetAmount(amount);
    }

    /// <summary>모든 셀을 제거하여 인벤토리를 초기화한다. contentParents의 각 부모 컨테이너에서 자식 오브젝트를 모두 파괴하고, cellMap을 비운다.</summary>
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