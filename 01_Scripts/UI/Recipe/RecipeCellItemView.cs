using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeCellItemView : MonoBehaviour
{
    [SerializeField] private Image image_icon;
    [SerializeField] private TextMeshProUGUI text_amount;

    public void Bind(Sprite icon, int amount)
    {
        image_icon.sprite = icon;
        text_amount.text = amount.ToString();
    }
}
