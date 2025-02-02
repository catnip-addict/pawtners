using UnityEngine;

public class TableScript : MonoBehaviour
{
    public TaskManager TaskManager;
    private void OnTriggerEnter(Collider other)
    {
        if (TaskManager.numerZadania == 1)
        {
            TaskManager.taskCompleted = true;
        }

    }
}
