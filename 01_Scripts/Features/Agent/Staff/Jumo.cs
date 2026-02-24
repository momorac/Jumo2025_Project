using UnityEngine;

[RequireComponent(typeof(Staff))]
public class Jumo : MonoBehaviour
{
    private Staff staff;
    void Awake()
    {
        staff = GetComponent<Staff>();
    }

    void Start()
    {
        if (App.StaffRegistry.GetDefaultStaff() == null)
        {
            App.StaffRegistry.SetDefaultStaff(staff);
        }
    }
}
