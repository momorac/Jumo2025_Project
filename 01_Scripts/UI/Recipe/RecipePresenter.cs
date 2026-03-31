using UnityEngine;

public class RecipePresenter : IPresenter
{
    private readonly RecipeView view;
    private readonly UIManager ui;
    private readonly WindowType windowType = WindowType.Recipe;

    private readonly IngredientService ingredientService;
    private readonly RecipeService recipeService;

    public RecipePresenter(RecipeView _view, UIManager _ui, IngredientService _ingredientService, RecipeService _recipeService)
    {
        this.view = _view;
        this.ui = _ui;
        this.ingredientService = _ingredientService;
        this.recipeService = _recipeService;
    }

    public void Initialize()
    {
        view.CloseButtonClicked += OnCloseClicked;
        BuildRecipeList();
    }

    public void Dispose()
    {
        view.CloseButtonClicked -= OnCloseClicked;
        view.ClearCells();
    }

    /// <summary> app의 RecipeService에서 잠금 해제된 레시피 목록을 가져와서, 각 레시피에 대해 BuildCell을 호출하여 UI에 표시한다 </summary>
    private void BuildRecipeList()
    {
        foreach (RecipeDefinition recipe in recipeService.GetUnlockedRecipes())
        {
            view.BuildCell(recipe);
            GameLogger.LogVerbose(LogCategory.UI, $"Added recipe cell for: {recipe.displayName}");
        }
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }
}
