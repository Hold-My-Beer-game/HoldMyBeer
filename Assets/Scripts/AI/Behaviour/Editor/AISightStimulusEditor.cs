#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HoldMyBeer.AI.Editor {
    [CustomEditor(typeof(AISightStimulus))]
    public sealed class AISightStimulusEditor : UnityEditor.Editor {
        private const int HorizontalSegments = 32;
        private const int VerticalSegments = 8;
        private static readonly Color ConeColor = new(0f, 1f, 0f);
        private static readonly Color NormalColor = Color.cyan;
        private static readonly Color HitColor = Color.red;

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Active)]
        private static void DrawGizmos(AISightStimulus sightStimulus, GizmoType gizmoType) {
            DrawSightCone(sightStimulus);
            foreach (AISightStimulus.SightBoxCastDebugData boxCast in sightStimulus.DebugBoxCasts) { DrawBoxCast(boxCast); }
        }

        private static void DrawSightCone(AISightStimulus sightStimulus) {
            Vector3 origin = sightStimulus.Origin;
            Color previousColor = Gizmos.color;

            Gizmos.color = ConeColor;
            Gizmos.DrawSphere(origin, 0.08f);

            int hSegments = Mathf.Max(2, HorizontalSegments);
            int vSegments = Mathf.Max(2, VerticalSegments);
            var points = new Vector3[hSegments + 1, vSegments + 1];

            for (int h = 0; h <= hSegments; h++) {
                float hT = h / (float)hSegments;
                float yaw = Mathf.Lerp(-sightStimulus.LeftAngle, sightStimulus.RightAngle, hT);

                for (int v = 0; v <= vSegments; v++) {
                    float vT = v / (float)vSegments;
                    float pitch = Mathf.Lerp(-sightStimulus.DownAngle, sightStimulus.UpAngle, vT);
                    points[h, v] = sightStimulus.GetConeEndPoint(yaw, pitch);
                }
            }

            for (int h = 0; h <= hSegments; h++) {
                for (int v = 0; v <= vSegments; v++) {
                    Vector3 point = points[h, v];
                    if (h < hSegments) { Gizmos.DrawLine(point, points[h + 1, v]); }
                    if (v < vSegments) { Gizmos.DrawLine(point, points[h, v + 1]); }
                }
            }
            DrawEdge(origin, points, 0, 0, hSegments, vSegments);

            Gizmos.color = previousColor;
        }

        private static void DrawEdge(Vector3 origin, Vector3[,] points, int minH, int minV, int maxH, int maxV) {
            Gizmos.DrawLine(origin, points[minH, minV]);
            Gizmos.DrawLine(origin, points[minH, maxV]);
            Gizmos.DrawLine(origin, points[maxH, minV]);
            Gizmos.DrawLine(origin, points[maxH, maxV]);
        }

        private static void DrawBoxCast(AISightStimulus.SightBoxCastDebugData boxCast) {
            Color previousColor = Gizmos.color;
            Matrix4x4 previousMatrix = Gizmos.matrix;

            Gizmos.color = boxCast.HitTarget ? HitColor : NormalColor;
            Gizmos.DrawLine(boxCast.Origin, boxCast.EndCenter);

            Gizmos.matrix = Matrix4x4.TRS(boxCast.EndCenter, boxCast.Rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxCast.HalfExtents * 2f);

            Gizmos.matrix = previousMatrix;
            Gizmos.color = previousColor;
        }
    }
}
#endif