using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform[] checkPoints;
    public void ResetToCheckPoint(Transform player, int checkPointIndex)
    {
        player.position = checkPoints[checkPointIndex].position;
        Debug.Log($"Player {checkPointIndex}");
    }
}
