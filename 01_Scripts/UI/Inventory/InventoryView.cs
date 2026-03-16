using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : WindowViewBase
{
    // [Header("Prefab References")]
    // [SerializeField] private GameObject inventoryItemCellPrefab;

    [Header("UI Elements")]
    [SerializeField] private Button button_close;

    public event Action CloseButtonClicked;

    private void Awake()
    {
        button_close.onClick.AddListener(() => CloseButtonClicked?.Invoke());
    }
}