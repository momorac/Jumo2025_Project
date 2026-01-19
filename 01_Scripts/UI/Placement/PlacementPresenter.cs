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
        view.ClearFacilityCells();
        view.CloseClicked -= OnCloseClicked;
    }

    private void AddAvailableFacility()
    {
        var availableFacilities = App.GameData.FacilityMetaData.GetUnlockedTypes();
        if (availableFacilities == null) return;

        foreach (var type in availableFacilities)
        {
            if (placementController.CanPlace(type))
            {
                var prefab = (placementController as PlacementController).GetUiIconPrefab(type);

                if (prefab == null)
                {
                    Debug.LogWarning($"No UI prefab found for facility type: {type}");
                    continue;
                }
                else
                {
                    view.AddFacilityCell(type, prefab);
                }
            }
        }
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }

}
