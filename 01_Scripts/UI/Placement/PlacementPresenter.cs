using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPresenter : IPresenter
{
    private readonly PlacementView view;
    private readonly UIManager ui;
    private readonly WindowType windowType = WindowType.Placement;

    private readonly IPlacementController controller;

    public PlacementPresenter(PlacementView view, UIManager ui, IPlacementController controller)
    {
        this.view = view;
        this.ui = ui;
        this.controller = controller;
    }

    public void Initialize()
    {
        // Debug.Log("PlacementPresenter Initialize");
        view.CloseClicked += OnCloseClicked;
        AddAvailableFacility();
    }

    public void Dispose()
    {
        // Debug.Log("PlacementPresenter Dispose");
        view.ClearCells();
        view.CloseClicked -= OnCloseClicked;
    }

    private void AddAvailableFacility()
    {
        var facilities = controller.GetAvailableFacilities();
        foreach (var facility in facilities)
        {
            Sprite icon = controller.GetUiIcon(facility);
            string name = controller.GetDisplayName(facility);
            view.AddPlaceableCell(new Facility(facility), name, icon, StartPlacing);
        }
    }

    private void StartPlacing(Placeable placeable)
    {
        ui.CloseWindow(windowType);
        controller.StartPlacing(placeable);
    }


    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }

}
