using UnityEngine;
using UnityEngine.UI;
using System;

public class PlacementView : WindowViewBase
{
    [Header("References")]
    [SerializeField] private GameObject placeableCellPrefab;

    [Header("UI Elements")]
    [SerializeField] private Button button_close;
    [SerializeField] private Transform content_parent;

    public event Action CloseClicked;

    private void Awake()
    {
        button_close.onClick.AddListener(() => CloseClicked?.Invoke());
    }

    public void AddPlaceableCell(Placeable placeable, Sprite icon)
    {
        var itemObj = Instantiate(placeableCellPrefab, content_parent);
        var itemView = itemObj.GetComponent<PlaceableCellView>();
        itemView.Bind(placeable, icon);
    }

    public void ClearCells()
    {
        if (content_parent == null) return;
        for (int i = content_parent.childCount - 1; i >= 0; i--)
        {
            var child = content_parent.GetChild(i);
            if (child != null) Destroy(child.gameObject);
        }
    }
}
