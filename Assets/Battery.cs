using UnityEngine;
public enum BatteryType
{
    Red,
    Blue,
}

public class Battery : MonoBehaviour
{
    public BatteryType batteryType;
    // public float charge = 100f; // Charge level from 0 to 100
    // public float dischargeRate = 1f; // Rate at which the battery discharges per second
    // public float rechargeRate = 2f; // Rate at which the battery recharges per second

    // private void Update()
    // {
    //     if (charge > 0)
    //     {
    //         charge -= dischargeRate * Time.deltaTime;
    //     }
    //     else
    //     {
    //         charge = 0;
    //     }
    // }

    // public void RechargeBattery(float amount)
    // {
    //     charge += amount * rechargeRate * Time.deltaTime;
    //     if (charge > 100f)
    //     {
    //         charge = 100f;
    //     }
    // }
    // public void DischargeBattery(float amount)
    // {
    //     charge -= amount * dischargeRate * Time.deltaTime;
    //     if (charge < 0f)
    //     {
    //         charge = 0f;
    //     }
    // }
}
