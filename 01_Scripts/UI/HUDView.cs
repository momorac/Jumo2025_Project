using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDView : WindowViewBase
{
    [SerializeField] private Button button_place;
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

}
