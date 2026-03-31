public class InventoryPresenter : IPresenter
{
    private readonly InventoryView view;
    private readonly UIManager ui;
    private readonly IngredientService ingredientService;
    private readonly WindowType windowType = WindowType.Inventory;

    public InventoryPresenter(InventoryView _view, UIManager _ui, IngredientService _ingredientService)
    {
        this.view = _view;
        this.ui = _ui;
        this.ingredientService = _ingredientService;
    }

    public void Initialize()
    {
        view.CloseButtonClicked += OnCloseClicked;
        // ingredientService.OnAmountChanged += OnAmountChanged;
        // ingredientService.OnIngredientUnlocked += OnIngredientUnlocked;
        BuildInventory();
    }

    public void Dispose()
    {
        view.CloseButtonClicked -= OnCloseClicked;
        // ingredientService.OnAmountChanged -= OnAmountChanged;
        // ingredientService.OnIngredientUnlocked -= OnIngredientUnlocked;
        view.ClearCells();
    }

    private void BuildInventory()
    {
        foreach (IngredientDefinition ingredient in ingredientService.GetAllDefinitions())
        {
            // if (!ingredientService.IsUnlocked(def.type)) continue;
            view.BuildCell(ingredient.type, ingredient.displayName, ingredient.icon, ingredientService.GetAmount(ingredient.type));
        }
    }

    private void OnAmountChanged(IngredientType type, int newAmount)
    {
        view.UpdateCellAmount(type, newAmount);
    }

    private void OnIngredientUnlocked(IngredientType type)
    {
        // Registry 순서 유지를 위해 전체 재빌드
        view.ClearCells();
        BuildInventory();
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }
}
