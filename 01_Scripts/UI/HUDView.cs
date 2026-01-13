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

    // HUD에 표시할 값이 있으면 최소로 Render만 추가
    public void Render(int money, int day)
    {
        // Text TMP 등 갱신
    }
}
