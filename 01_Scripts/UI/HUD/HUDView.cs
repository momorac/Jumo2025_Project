using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : WindowViewBase
{
    [SerializeField] private Button button_place;
    [SerializeField] private Button button_recipe;
    [SerializeField] private Button button_inventory;
    [SerializeField] private TextMeshProUGUI text_money;

    public event Action PlaceClicked;
    public event Action RecipeClicked;
    public event Action InventoryClicked;

    private void Awake()
    {
        button_place.onClick.AddListener(() => PlaceClicked?.Invoke());
        button_recipe.onClick.AddListener(() => RecipeClicked?.Invoke());
        button_inventory.onClick.AddListener(() => InventoryClicked?.Invoke());
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdateMoney(int amount)
    {
        text_money.text = amount.ToString();
    }

}
