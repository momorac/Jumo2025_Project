using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableCellView : MonoBehaviour, IView
{
    [SerializeField] private TextMeshProUGUI text_name;
    [SerializeField] private Image image_icon;

    private Placeable type;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Bind(Placeable type, Sprite icon)
    {
        this.type = type;
        // text_name.text = type.GetDisplayName();
        image_icon.sprite = icon;
    }

}
