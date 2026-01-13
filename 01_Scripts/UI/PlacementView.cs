using UnityEngine;
using UnityEngine.UI;
using System;

public class PlacementView : WindowViewBase
{
    [SerializeField] private Button button_close;
    public event Action CloseClicked;

    private void Awake()
    {
        button_close.onClick.AddListener(() => CloseClicked?.Invoke());
    }



}
