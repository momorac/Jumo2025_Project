using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 테이블 청소 작업 (1 Phase)
/// 테이블로 이동 → 청소
/// </summary>
public class CleanTableTask : StaffTaskBase
{
    public override TaskType Type => TaskType.CleanTable;

    private readonly Transform tablePosition;

    public CleanTableTask(Transform tablePosition, int priority = 5)
        : base(priority)
    {
        this.tablePosition = tablePosition;
    }

    protected override List<TaskPhase> BuildPhases() => new()
    {
        new TaskPhase(
            moveTarget: tablePosition,
            duration: 3f,
            animationTrigger: "CleanTable",
            onExecute: staff =>
            {
                GameLogger.LogVerbose(LogCategory.Task, $"{staff.name} cleaning table");
            }
        )
    };
}
