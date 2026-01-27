using UnityEngine;

public interface IPooled
{
    void OnGet();
    void OnRelease();
}
