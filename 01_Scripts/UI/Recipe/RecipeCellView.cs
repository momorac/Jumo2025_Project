using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 레시피 셀 뷰 스크립트
/// </summary>
public class RecipeCellView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI text_recipeName;
    [SerializeField] private Image image_icon;
    [SerializeField] private TextMeshProUGUI text_price;

    [Space(10)]
    [SerializeField] private RectTransform ingredientListParent;
    [SerializeField] private RecipeCellItemView ingredientItemPrefab;

    private RecipeDefinition recipe;

    /// <summary> 레시피 데이터를 받아와서 UI 요소에 바인딩한다. 레시피 이름, 아이콘, 가격을 표시하고, 재료 목록을 생성하여 표시한다. </summary>
    public void Bind(RecipeDefinition _recipe)
    {
        recipe = _recipe;

        // 기본 레시피 정보 표시
        text_recipeName.text = recipe.displayName;
        image_icon.sprite = recipe.icon;
        text_price.text = recipe.defaultPrice.ToString();

        // 레시피 재료 목록 생성 및 표시
        foreach (RecipeIngredient ingredient in recipe.ingredients)
        {
            RecipeCellItemView itemView = Instantiate(ingredientItemPrefab, ingredientListParent);
            itemView.Bind(GetIngredientIcon(ingredient.ingredient), ingredient.amount);
        }
    }

    /// <summary> 재료 유형에 해당하는 아이콘을 IngredientService에서 가져온다. </summary>
    private Sprite GetIngredientIcon(IngredientType ingredient)
    {
        IngredientDefinition def = App.IngredientService.GetDefinition(ingredient);
        return def.icon;
    }
}
