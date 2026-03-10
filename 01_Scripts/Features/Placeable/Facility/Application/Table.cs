using System.Collections.Generic;
using UnityEngine;


public class Table : MonoBehaviour, IFacility
{
    public Placeable Type { get; private set; }

    [SerializeField] private Seat[] seatRoots;
    public int Capacity => seatRoots.Length;

    public void Initialize(Placeable _placeable)
    {
        this.Type = _placeable;
    }

    private void RegisterTable()
    {
        for (int i = 0; i < seatRoots.Length; i++)
        {
            App.SessionService.RegisterSeat(seatRoots[i]);
        }
    }

    public void OnPlaced()
    {
        GameLogger.Log(LogCategory.Facility, $"Table placed. Capacity: {Capacity}");

        // 모델 오브젝트 y축 중심으로 90도씩 랜덤하게 회전
        int randomRotation = Random.Range(0, 4) * 90;
        transform.GetChild(0).rotation = Quaternion.Euler(0, randomRotation, 0);

        RegisterTable();
    }

    public void OnRemoved()
    {
    }
}

[System.Serializable]
public class Seat
{
    public Transform Root;
    public List<Transform> MotionRoots;
}
