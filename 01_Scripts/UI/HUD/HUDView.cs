using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : WindowViewBase
{
    [SerializeField] private Button button_place;
    [SerializeField] private TextMeshProUGUI text_money;

    public event Action PlaceClicked;

    private void Awake()
    {
        button_place.onClick.AddListener(() => PlaceClicked?.Invoke());
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
