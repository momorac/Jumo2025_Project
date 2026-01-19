using UnityEngine;

public abstract class WindowViewBase : MonoBehaviour, IView
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
