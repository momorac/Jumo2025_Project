using System;
using UnityEngine;
using UnityEngine.UI;

public class RecipeView : WindowViewBase
{
    [Header("Prefab References")]
    [SerializeField] private RecipeCellView recipeCellPrefab;

    [Header("UI Elements")]
    [SerializeField] private Button button_close;
    [SerializeField] private RectTransform rt_content;


    public event Action CloseButtonClicked;

    private void Awake()
    {
        button_close.onClick.AddListener(() => CloseButtonClicked?.Invoke());
    }

    /// <summary> 레시피 셀을 생성하여 rt_content에 추가한다. 셀은 RecipeDefinition으로 식별되며, 레시피 이름을 표시한다.</summary>
    public void BuildCell(RecipeDefinition recipe)
    {
        RecipeCellView cellView = Instantiate(recipeCellPrefab, rt_content);
        cellView.Bind(recipe);
    }
}
