using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
            WireCube,
            Sphere,
            WireSphere,
        }

        public enum RayDirection
        {
            Up,
            Down,
            Left,
            Right,
            Forward,
            Back
        }

        public GizmoShape gizmoShape;
        [EnumButtons] public RayDirection rayDirection;
        public bool useLocalSpace;
        public Color gizmoColor = Color.white;
        public Vector3 gizmoOffset = Vector3.zero;
        public float gizmoScale = 1f;
        public bool matchObjectLocalScale;
        private Vector3 _direction = Vector3.zero;

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;

            _direction = rayDirection switch
            {
                RayDirection.Up => useLocalSpace ? gameObject.transform.up : Vector3.up,
                RayDirection.Down => useLocalSpace ? -gameObject.transform.up : Vector3.down,
                RayDirection.Right => useLocalSpace ? gameObject.transform.right : Vector3.right,
                RayDirection.Left => useLocalSpace ? -gameObject.transform.right : Vector3.left,
                RayDirection.Forward => useLocalSpace ? gameObject.transform.forward : Vector3.forward,
                RayDirection.Back => useLocalSpace ? -gameObject.transform.forward : Vector3.back,
                _ => throw new ArgumentOutOfRangeException()
            };

            switch (gizmoShape)
            {
                case GizmoShape.Ray:
                    Gizmos.DrawRay(transform.position + gizmoOffset, matchObjectLocalScale ? _direction * transform.localScale.z : _direction * gizmoScale);
                    break;
                case GizmoShape.Line:
                    Gizmos.DrawLine(transform.position + gizmoOffset, new Vector3(0, 0, 0));
                    break;
                case GizmoShape.Cube:
                    Gizmos.DrawCube(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale : new Vector3(gizmoScale, gizmoScale, gizmoScale));
                    break;
                case GizmoShape.WireCube:
                    Gizmos.DrawWireCube(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale : new Vector3(gizmoScale, gizmoScale, gizmoScale));
                    break;
                case GizmoShape.Sphere:
                    Gizmos.DrawSphere(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale.y : gizmoScale);
                    break;
                case GizmoShape.WireSphere:
                    Gizmos.DrawWireSphere(transform.position + gizmoOffset, matchObjectLocalScale ? transform.localScale.y : gizmoScale);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}