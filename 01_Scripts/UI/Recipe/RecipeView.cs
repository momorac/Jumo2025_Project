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

    /// <summary> 모든 셀을 제거하여 레시피 목록을 초기화한다. rt_content의 자식 오브젝트를 모두 파괴한다.</summary>
    public void ClearCells()
    {
        for (int i = rt_content.childCount - 1; i >= 0; i--)
        {
            Destroy(rt_content.GetChild(i).gameObject);
        }
    }
}
