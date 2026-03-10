using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 요리 작업 (1 Phase)
/// 조리 시설로 이동 → 요리 모션 → 레시피 재료 소비 + 시설 자원(물/장작) 소비
/// </summary>
public class CookTask : StaffTaskBase
{
    public override TaskType Type => TaskType.Cook;

    public CookingFacilityBase Facility { get; }
    public RecipeType RecipeType { get; }

    public CookTask(RecipeType recipeType, CookingFacilityBase facility, int priority = 5)
        : base(priority)
    {
        RecipeType = recipeType;
        Facility = facility;
    }

    protected override List<TaskPhase> BuildPhases()
    {
        var definition = App.RecipeService.GetDefinition(RecipeType);
        float duration = definition != null ? definition.cookingTime : 3f;

        return new List<TaskPhase>
        {
            new TaskPhase(
                moveTarget: Facility.TargetTransform,
                duration: duration,
                animationTrigger: "Cook",
                onStart: (controller) =>
                {
                    controller.StopMoving();
                    Quaternion targetRotation = Quaternion.LookRotation(Facility.transform.position - controller.transform.position);
                    targetRotation *= Quaternion.Euler(0f, -90f, 0f);
                    controller.SetPositionAndRotation(controller.transform.position, targetRotation);
                },
                onExecute: (controller) =>
                {
                    App.RecipeService.Cook(RecipeType);
                    Facility.ConsumeResources();
                    GameLogger.Log(LogCategory.Task, $"[CookTask] {RecipeType} 요리 완료 at {Facility.name}");
                }
            )
        };
    }
}
