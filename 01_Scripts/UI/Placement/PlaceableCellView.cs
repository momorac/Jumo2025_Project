using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableCellView : MonoBehaviour, IView
{
    [SerializeField] private TextMeshProUGUI text_name;
    [SerializeField] private Image image_icon;
    [SerializeField] private Button button_select;

    private Placeable type;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Bind(Placeable type, Sprite icon, Action<Placeable> onClick)
    {
        this.type = type;
        text_name.text = type.GetDisplayName();
        image_icon.sprite = icon;
        button_select.onClick.RemoveAllListeners();
        button_select.onClick.AddListener(() => onClick?.Invoke(this.type));
    }

}
