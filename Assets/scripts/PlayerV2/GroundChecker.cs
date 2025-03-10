using UnityEngine;

namespace Platformer
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float groundDistance = 0.08f;
        [SerializeField] LayerMask groundLayers;
        [SerializeField] CapsuleCollider capsulecollider;


        public bool IsGrounded { get; private set; }
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float radius = capsulecollider.radius * 1.2f;

            // Use transform.TransformPoint to rotate positions relative to the player
            Vector3 pos1 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, radius * 2));
            Vector3 pos2 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, -radius * 2));

            Gizmos.DrawWireSphere(pos1, radius);
            Gizmos.DrawWireSphere(pos2, radius);
        }

        void Update()
        {
            float radius = capsulecollider.radius * 1.2f;

            // Use the same transformations for the actual ground check
            Vector3 pos1 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, radius * 2));
            Vector3 pos2 = transform.TransformPoint(new Vector3(0, -radius * 1.2f, -radius * 2));

            IsGrounded = Physics.CheckSphere(pos1, radius, groundLayers) ||
                         Physics.CheckSphere(pos2, radius, groundLayers);
        }
    }
}