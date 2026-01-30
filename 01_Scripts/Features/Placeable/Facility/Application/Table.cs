using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour, IFacilityService
{
    public Placeable Type { get; private set; }

    [SerializeField] private Transform[] seatRoot;
    public int Capacity => seatRoot.Length;

    public void Initialize(Placeable _placeable)
    {
        this.Type = _placeable;
    }

    private void RegisterTable()
    {
        for (int i = 0; i < seatRoot.Length; i++)
        {
            App.SessionService.RegisterSeat(seatRoot[i]);
        }
    }

    public void OnPlaced()
    {
        Debug.Log($"<color=green>Table placed. Capacity: {Capacity}.</color>");

        // 모델 오브젝트 y축 중심으로 90도씩 랜덤하게 회전
        int randomRotation = Random.Range(0, 4) * 90;
        transform.GetChild(0).rotation = Quaternion.Euler(0, randomRotation, 0);

        RegisterTable();
    }

    public void OnRemoved()
    {
    }
}
