using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 활성화된 모든 Staff를 추적하는 레지스트리.
/// TaskAssigner가 배정 시 활용
/// </summary>
public class StaffRegistry
{
    private readonly List<Staff> registeredStaffs = new();
    private Staff selectedStaff;
    private Staff defaultStaff;

    public void Register(Staff staff)
    {
        if (!registeredStaffs.Contains(staff))
        {
            registeredStaffs.Add(staff);
            GameLogger.LogVerbose(LogCategory.Staff, $"Registered: {staff.name}. Total: {registeredStaffs.Count}");
        }
    }

    public void Unregister(Staff staff)
    {
        if (registeredStaffs.Contains(staff))
        {
            registeredStaffs.Remove(staff);

            if (selectedStaff == staff)
            {
                selectedStaff = null;
            }

            GameLogger.LogVerbose(LogCategory.Staff, $"Unregistered: {staff.name}. Total: {registeredStaffs.Count}");
        }
    }

    /// <summary>모든 등록된 Staff 반환</summary>
    public IReadOnlyList<Staff> GetAllStaffs() => registeredStaffs.AsReadOnly();

    /// <summary>Idle 상태인 모든 Staff 반환</summary>
    public List<Staff> GetIdleStaffs()
    {
        return registeredStaffs.Where(s => s.IsIdle).ToList();
    }

    /// <summary>특정 위치에서 가장 가까운 Idle Staff 반환</summary>
    public Staff GetClosestIdleStaff(Vector3 position)
    {
        return GetIdleStaffs()
            .OrderBy(s => Vector3.Distance(s.transform.position, position))
            .FirstOrDefault();
    }

    /// <summary>Staff 선택</summary>
    public void SelectStaff(Staff staff)
    {
        selectedStaff = staff;
        App.EventBus.Publish(new StaffSelectedEvent(staff));
    }

    /// <summary>선택 해제</summary>
    public void ClearSelection()
    {
        selectedStaff = null;
    }

    /// <summary>현재 선택된 Staff 반환</summary>
    public Staff GetSelectedStaff() => selectedStaff;

    /// <summary>Staff가 선택되어 있는지 확인</summary>
    public bool HasSelection => selectedStaff != null;

    /// <summary>Staff 수 반환</summary>
    public int Count => registeredStaffs.Count;

    /// <summary>기본 Staff(주모) 설정</summary>
    public void SetDefaultStaff(Staff staff)
    {
        defaultStaff = staff;
        GameLogger.LogVerbose(LogCategory.Staff, $"Default staff set: {staff.name}");
    }

    /// <summary>기본 Staff(주모) 반환</summary>
    public Staff GetDefaultStaff() => defaultStaff;
}
