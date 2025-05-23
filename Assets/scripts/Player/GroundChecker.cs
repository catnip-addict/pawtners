﻿using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] float groundDistance = 0.08f;
    [SerializeField] LayerMask groundLayers;
    [SerializeField] CapsuleCollider capsulecollider;
    LayerMask InsideLayers;


    public bool IsGrounded { get; private set; }
    void Start()
    {
        InsideLayers = LayerMask.GetMask("Ground");
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float radius = capsulecollider.radius * 1.2f;

        // Use transform.TransformPoint to rotate positions relative to the player
        Vector3 pos1 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, radius * 2));
        Vector3 pos2 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, -radius * 2));
        Vector3 pos3 = transform.TransformPoint(new Vector3(0, radius * 0.1f, radius * 0.1f));

        Gizmos.DrawWireSphere(pos1, radius);
        Gizmos.DrawWireSphere(pos2, radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos3, radius * 0.9f);
    }

    void Update()
    {
        float radius = capsulecollider.radius * 1.2f;

        // Use the same transformations for the actual ground check
        Vector3 pos1 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, radius * 2));
        Vector3 pos2 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, -radius * 2));
        Vector3 pos3 = transform.TransformPoint(new Vector3(0, radius * 0.1f, radius * 0.1f));

        IsGrounded = Physics.CheckSphere(pos1, radius, groundLayers) ||
                     Physics.CheckSphere(pos2, radius, groundLayers);
        if (Physics.CheckSphere(pos3, radius * 0.9f, InsideLayers))
        {
            // Debug.Log(groundLayers.value);
            GetComponent<Player>().SetJumpVelocity(5);
        }
    }
}