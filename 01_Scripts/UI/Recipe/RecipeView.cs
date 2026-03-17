using System;
using UnityEngine;
using UnityEngine.UI;

public class RecipeView : WindowViewBase
{
    // [Header("Prefab References")]
    // Prefab references for item or cells can be added here

    // [Header("UI Elements")]
    // References to UI elements (e.g., buttons, panels) can be added here

    public event Action CloseButtonClicked;

    private void Awake()
    {
        // Add listeners to UI elements here
    }
}
