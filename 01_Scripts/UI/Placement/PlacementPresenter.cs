using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPresenter : IPresenter
{
    private readonly PlacementView view;
    private readonly UIManager ui;
    private readonly WindowType windowType = WindowType.Placement;

    private readonly IPlacementController placementController;

    public PlacementPresenter(PlacementView view, UIManager ui, IPlacementController placementController)
    {
        this.view = view;
        this.ui = ui;
        this.placementController = placementController;
    }

    public void Initialize()
    {
        Debug.Log("PlacementPresenter Initialize");
        view.CloseClicked += OnCloseClicked;
        AddAvailableFacility();
    }

    public void Dispose()
    {
        Debug.Log("PlacementPresenter Dispose");
        view.ClearCells();
        view.CloseClicked -= OnCloseClicked;
    }

    private void AddAvailableFacility()
    {
        var facilities = placementController.GetAvailableFacilities();
        foreach (var facility in facilities)
        {
            GameObject prefab = placementController.GetUiIconPrefab(facility);
            view.AddPlaceableCell(new Facility(facility), prefab);
        }
    }


    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }

}
