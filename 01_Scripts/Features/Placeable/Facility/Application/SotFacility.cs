using UnityEngine;

/// <summary> 솥 or </summary>
public class SotFacility : CookingFacilityBase
{
    private void Awake()
    {
        facilityType = FacilityType.Sot;
    }

    public override void OnClicked(Vector3 hitPoint)
    {
        App.EventBus.Publish(new CookingFacilityClickedEvent(this));

        GameLogger.Log(LogCategory.Input, $"Sot clicked (CanCook: {CanCook})");

        if (CanCook)
        {
            var recipe = App.RecipeService.GetCookableRecipes().Find(d => d.CanCookAt(CookingType));
            if (recipe != null)
            {
                App.TaskQueue.Enqueue(new CookTask(recipe.type, this, priority: 5));
            }
            else
            {
                App.TaskQueue.Enqueue(new CookTask(RecipeType.Default, this, priority: 5)); // 기본 레시피로 대체
                // GameLogger.LogWarning(LogCategory.Task, "No cookable recipe available for this facility");
            }
        }
    }
}
