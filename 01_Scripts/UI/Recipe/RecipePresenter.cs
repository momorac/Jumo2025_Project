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
    }

    private void BuildRecipeList()
    {
        foreach (RecipeDefinition recipe in recipeService.GetUnlockedRecipes())
        {
            view.BuildCell(recipe);
        }
    }

    private void OnCloseClicked()
    {
        ui.CloseWindow(windowType);
    }
}
