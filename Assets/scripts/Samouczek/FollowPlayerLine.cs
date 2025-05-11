using System.Collections.Generic;
using UnityEngine;
public class FollowPlayerLine : MonoBehaviour
{
    public Transform player;
    // private List<Vector3> points = new List<Vector3>();
    public Vector3 offset = new Vector3(0, 1, 0);

    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        Debug.Log(line.positionCount);

    }

    void Update()
    {
        line.SetPosition(line.positionCount - 1, player.position + offset);
    }
    public void RemoveLastPoint(int newCount)
    {

        if (line.positionCount > 0)
        {
            // int newCount = line.positionCount - 1;
            Vector3[] newPositions = new Vector3[newCount];

            for (int i = 0; i < newCount; i++)
            {
                newPositions[i] = line.GetPosition(i);
            }

            line.positionCount = newCount;
            line.SetPositions(newPositions);
        }
    }
}
