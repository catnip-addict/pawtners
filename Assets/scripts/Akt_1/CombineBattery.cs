using UnityEngine;
using UnityEngine.Events;
public class CombineBattery : MonoBehaviour
{
    public UnityEvent onButtonPress;
    bool red = false;
    bool blue = false;
    public void PutBatteryIn(int batteryType)
    {
        if (batteryType == 0)
        {
            red = true;
            Debug.Log("Red Battery Inserted!");
        }
        else if (batteryType == 1)
        {
            blue = true;
            Debug.Log("Blue Battery Inserted!");
        }
        if (red && blue)
        {
            Debug.Log("Battery Combined!");
            onButtonPress.Invoke();
            gameObject.SetActive(false);
        }
    }
}
