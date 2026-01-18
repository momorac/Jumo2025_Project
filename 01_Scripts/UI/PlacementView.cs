using UnityEngine;
using UnityEngine.UI;
using System;

public class PlacementView : WindowViewBase
{
    [SerializeField] private Button button_close;
    [SerializeField] private Transform content_parent;


    public event Action CloseClicked;

    private void Awake()
    {
        button_close.onClick.AddListener(() => CloseClicked?.Invoke());
    }

    public void AddFacilityCell(FacilityType type, GameObject prefab)
    {
        var itemObj = Instantiate(prefab, content_parent);
        var itemView = itemObj.GetComponent<PlacementCellView>();
        itemView.Bind(type);
    }

    public void ClearFacilityCells()
    {
        if (content_parent == null) return;
        for (int i = content_parent.childCount - 1; i >= 0; i--)
        {
            var child = content_parent.GetChild(i);
            if (child != null) Destroy(child.gameObject);
        }
    }
}
