using UnityEngine;

public class PlaceableCellView : MonoBehaviour, IView
{


    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Bind(Placeable type)
    {
        // Bind data to UI elements here
    }

}
