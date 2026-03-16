
using UnityEngine;

public class InventoryPresenter : IPresenter
{
    private readonly InventoryView view;
    private readonly UIManager ui;
    private readonly WindowType windowType = WindowType.Inventory;

    public InventoryPresenter(InventoryView view, UIManager ui)
    {
        this.view = view;
        this.ui = ui;
    }

    public void Initialize()
    {
        view.CloseButtonClicked += OnCloseClicked;
    }

    public void Dispose()
    {
        view.CloseButtonClicked -= OnCloseClicked;
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }
}
