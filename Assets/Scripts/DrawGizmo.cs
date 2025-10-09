using UnityEngine;

namespace Ske131
{
    [AddComponentMenu("Parkour Game/DrawGizmo")]
    public class DrawGizmo : MonoBehaviour
    {
        public enum GizmoShape
        {
            Ray,
            Line,
            Cube,
            Wirecube,
            Sphere,
            Wiresphere,
        }

        public GizmoShape gizmoShape;
        public Color gizmoColor = Color.white;
        public Vector3 gizmoOffset = Vector3.zero;
        public float gizmoScale = 1f;
        public bool matchObjectLocalScale;

        void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;

            switch (gizmoShape)
            {
                case GizmoShape.Ray:
                    Gizmos.DrawRay(transform.position + gizmoOffset, matchObjectLocalScale ? transform.forward * transform.localScale.z : transform.forward * gizmoScale);
                    break;
                case GizmoShape.Line:
                    Gizmos.DrawLine(transform.position + gizmoOffset, new Vector3(0, 0, 0));
                    break;
                case GizmoShape.Cube:
                    Gizmos.DrawCube(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale : new Vector3(gizmoScale, gizmoScale, gizmoScale));
                    break;
                case GizmoShape.Wirecube:
                    Gizmos.DrawWireCube(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale : new Vector3(gizmoScale, gizmoScale, gizmoScale));
                    break;
                case GizmoShape.Sphere:
                    Gizmos.DrawSphere(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale.y : gizmoScale);
                    break;
                case GizmoShape.Wiresphere:
                    Gizmos.DrawWireSphere(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale.y : gizmoScale);
                    break;
                default:
                    break;

            }
        }
    }
}