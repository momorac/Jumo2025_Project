public class HUDPresenter : IPresenter
{
    private readonly HUDView view;
    private readonly UIManager ui;

    public HUDPresenter(HUDView view, UIManager ui)
    {
        this.view = view;
        this.ui = ui;

        Initialize();
    }

    public void Initialize()
    {
        view.PlaceClicked += OnPlaceClicked;
        view.RecipeClicked += OnRecipeClicked;
        view.InventoryClicked += OnInventoryClicked;

        App.EconomyService.OnMoneyChanged += OnMoneyChanged;
        view.UpdateMoney(App.EconomyService.GetMoney());
    }

    private void OnMoneyChanged(int newAmount)
    {
        // 구현
        view.UpdateMoney(newAmount);
    }

    private void OnPlaceClicked() => ui.OpenWindow(WindowType.Placement);
    private void OnRecipeClicked() => ui.OpenWindow(WindowType.Recipe);
    private void OnInventoryClicked() => ui.OpenWindow(WindowType.Inventory);


    public void Dispose()
    {
        view.PlaceClicked -= OnPlaceClicked;
        view.RecipeClicked -= OnRecipeClicked;
        view.InventoryClicked -= OnInventoryClicked;
    }
}
