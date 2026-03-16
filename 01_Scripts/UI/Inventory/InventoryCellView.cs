using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 인벤토리 셀 스크립트가 구현해야 하는 바인딩 계약.
/// 직접 제작하는 셀 프리팹의 스크립트에서 이 인터페이스를 구현한다.
/// </summary>
public class InventoryCellView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image image_icon;
    [SerializeField] private TextMeshProUGUI text_name;
    [SerializeField] private TextMeshProUGUI text_amount;

    private IngredientType ingredientType;

    /// <summary>최초 렌더 시 재료 메타데이터와 수량을 한 번에 바인딩</summary>
    public void Bind(IngredientType type, string displayName, Sprite icon, int amount)
    {
        ingredientType = type;
        image_icon.sprite = icon;
        text_name.text = displayName;
        text_amount.text = amount.ToString();
    }

    /// <summary>보유량만 갱신 (수량 변경 이벤트 수신 시 호출)</summary>
    public void SetAmount(int amount)
    {
        text_amount.text = amount.ToString();
    }
}
