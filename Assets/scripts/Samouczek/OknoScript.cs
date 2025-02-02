using UnityEngine;

public class OknoScript : MonoBehaviour
{
    public TaskManager TaskManager;
    private void OnTriggerEnter(Collider other)
    {
        if (TaskManager.numerZadania == 4)
        {
            TaskManager.taskCompleted = true;
        }

    }
}
