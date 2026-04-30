using System;
using System.Collections.Generic;
using UnityEngine;

namespace CocaCopa.StateMachine {
    /// <summary>
    /// Detects whether a target collider is visible inside a configurable sight cone using multiple box casts.
    /// Also exposes editor-only debug data for visualization.
    /// </summary>
    public class AISightStimulus : MonoBehaviour {
        [Tooltip("Maximum distance at which a target can be detected.")]
        [SerializeField] private float sightDistance = 10f;
        [Tooltip("Horizontal sight angle to the left of the forward direction, in degrees.")]
        [SerializeField] private float leftAngle = 45f;
        [Tooltip("Horizontal sight angle to the right of the forward direction, in degrees.")]
        [SerializeField] private float rightAngle = 45f;
        [Tooltip("Vertical sight angle above the forward direction, in degrees.")]
        [SerializeField] private float upAngle = 30f;
        [Tooltip("Vertical sight angle below the forward direction, in degrees.")]
        [SerializeField] private float downAngle = 30f;

        [Tooltip("Local-space offset used as the origin point for all sight checks.")]
        [SerializeField] private Vector3 originOffset = Vector3.zero;
        [Tooltip("Layer mask used to determine which objects block or receive sight checks.")]
        [SerializeField] private LayerMask sightMask = ~0;
        [Tooltip("Specifies whether trigger colliders should be considered during sight checks.")]
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
        [Tooltip("Depth of each box cast used to sample visibility.")]
        [SerializeField] private float visionDepth = 0.02f;
        [Tooltip("Number of Horizontal (X) and Vertical (Y) samples taken across the target collider.")]
        [SerializeField] private Vector2 visionPointsCount;
        [Tooltip("Percentage size of each box cast relative to its sampled cell on the target.")]
        [SerializeField] [Range(1, 100)] private float visionPointSize = 10f;

        private static readonly string ScriptName = $"[{nameof(AISightStimulus)}]";

        /// <summary>
        /// World-space origin point used for sight calculations.
        /// </summary>
        public Vector3 Origin => transform.TransformPoint(originOffset);

#if UNITY_EDITOR
        /// <summary>Left horizontal sight angle in degrees.</summary>
        public float LeftAngle => leftAngle;

        /// <summary>Right horizontal sight angle in degrees.</summary>
        public float RightAngle => rightAngle;

        /// <summary>Upward vertical sight angle in degrees.</summary>
        public float UpAngle => upAngle;

        /// <summary>Downward vertical sight angle in degrees.</summary>
        public float DownAngle => downAngle;

        private readonly List<SightBoxCastDebugData> debugBoxCasts = new();

        /// <summary>
        /// Collection of debug data for all box casts performed during the last visibility check.
        /// </summary>
        public IReadOnlyList<SightBoxCastDebugData> DebugBoxCasts => debugBoxCasts;
#endif

        /// <summary>
        /// Determines whether the given collider is visible within the sight cone using multiple box casts.
        /// </summary>
        /// <param name="targetCollider">The collider to test for visibility.</param>
        /// <returns>True if the target is visible; otherwise false.</returns>
        public bool CanSee(Collider targetCollider) {
            if (!targetCollider) { throw new ArgumentException($"{ScriptName} {nameof(targetCollider)}"); }
#if UNITY_EDITOR
            debugBoxCasts.Clear();
#endif
            bool success = false;
            Vector3 toTarget = targetCollider.ClosestPoint(Origin) - Origin;
            if (toTarget.sqrMagnitude > sightDistance * sightDistance) { return false; }

            Vector3 relativeRight = GetRelativeRight(transform, targetCollider.transform);
            Vector3 relativeUp = targetCollider.transform.up;
            Vector3 targetSize = targetCollider.bounds.size;

            float horizontalPoints = visionPointsCount.x;
            float verticalPoints = visionPointsCount.y;

            float horizontalStep = targetSize.x / horizontalPoints;
            float verticalStep = targetSize.y / verticalPoints;

            Vector3 size = new(horizontalStep * visionPointSize / 100f, verticalStep * visionPointSize / 100f, visionDepth);
            Vector3 startPos = targetCollider.bounds.center;
            startPos += relativeRight * (horizontalStep / 2f - targetSize.x / 2f);
            startPos += relativeUp * (targetSize.y / 2f - verticalStep / 2f);

            for (int i = 0; i < verticalPoints; i++) {
                Vector3 rowStart = startPos - i * verticalStep * relativeUp;
                for (int j = 0; j < horizontalPoints; j++) {
                    Vector3 onRowPos = rowStart + j * horizontalStep * relativeRight;
                    Vector3 toRowDir = (onRowPos - Origin).normalized;

                    float castDistance = sightDistance;
                    bool hitCol = Physics.BoxCast(Origin, size, toRowDir, out RaycastHit hit, GetSightBoxRotation(toRowDir), castDistance, sightMask, triggerInteraction);
                    bool hitTarget = ReferenceEquals(hit.collider, targetCollider) && IsPointInsideSightCode(hit.point);

                    if (!hitCol) { continue; }
                    if (hitTarget) { success = true; }

                    float debugCastDistance = hit.distance;
#if UNITY_EDITOR
                    debugBoxCasts.Add(new SightBoxCastDebugData(Origin, toRowDir, GetSightBoxRotation(toRowDir), size / 2f, debugCastDistance, hitTarget));
#endif
                }
            }
            return success;
        }

        /// <summary>
        /// Checks whether a given world-space point lies inside the configured sight cone.
        /// </summary>
        /// <param name="targetPosition">World-space position to test.</param>
        /// <returns>True if the point is inside the sight cone; otherwise false.</returns>
        private bool IsPointInsideSightCode(Vector3 targetPosition) {
            Vector3 directionToTarget = targetPosition - Origin;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget > sightDistance || distanceToTarget < 0.001f) { return false; }

            Vector3 localDir = Quaternion.Inverse(transform.rotation) * directionToTarget.normalized;

            float yaw = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
            float flatDistance = Mathf.Sqrt(localDir.x * localDir.x + localDir.z * localDir.z);
            float pitch = Mathf.Atan2(localDir.y, flatDistance) * Mathf.Rad2Deg;

            return yaw >= -leftAngle && yaw <= rightAngle && pitch >= -downAngle && pitch <= upAngle;
        }

        /// <summary>
        /// Calculates the right vector relative to the direction from one transform to another on the horizontal plane.
        /// </summary>
        /// <param name="from">Origin transform.</param>
        /// <param name="to">Target transform.</param>
        /// <returns>Normalized right direction vector.</returns>
        private static Vector3 GetRelativeRight(Transform from, Transform to) {
            Vector3 targetPos = to.position;
            targetPos.y = 0f;
            Vector3 myPos = from.position;
            myPos.y = 0f;
            Vector3 toTargetDir = (targetPos - myPos).normalized;

            return Vector3.Cross(from.up, toTargetDir);
        }

        /// <summary>
        /// Calculates the rotation used for a box cast based on its direction.
        /// </summary>
        /// <param name="direction">Forward direction of the cast.</param>
        /// <returns>Rotation aligned with the given direction.</returns>
        private Quaternion GetSightBoxRotation(Vector3 direction) {
            if (direction.sqrMagnitude <= 0.0001f) { return transform.rotation; }

            return Quaternion.LookRotation(direction, transform.up);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Calculates a world-space point on the edge of the sight cone for visualization purposes.
        /// </summary>
        /// <param name="yaw">Horizontal angle offset in degrees.</param>
        /// <param name="pitch">Vertical angle offset in degrees.</param>
        /// <returns>World-space end point at the configured sight distance.</returns>
        public Vector3 GetConeEndPoint(float yaw, float pitch) {
            Quaternion rotation = transform.rotation * Quaternion.Euler(-pitch, yaw, 0f);
            Vector3 direction = rotation * Vector3.forward;

            return Origin + direction * sightDistance;
        }

        /// <summary>
        /// Debug data representing a single box cast used during sight checks.
        /// </summary>
        public readonly struct SightBoxCastDebugData {
            /// <summary>World-space origin of the cast.</summary>
            public readonly Vector3 Origin;
            /// <summary>Direction of the cast.</summary>
            public readonly Vector3 Direction;
            /// <summary>Rotation of the box used in the cast.</summary>
            public readonly Quaternion Rotation;
            /// <summary>Half extents of the box used in the cast.</summary>
            public readonly Vector3 HalfExtents;
            /// <summary>Distance the cast traveled before hitting something.</summary>
            public readonly float Distance;
            /// <summary>Indicates whether the cast hit the intended target.</summary>
            public readonly bool HitTarget;

            /// <summary>
            /// Creates a new debug data instance for a box cast.
            /// </summary>
            public SightBoxCastDebugData(Vector3 origin, Vector3 direction, Quaternion rotation, Vector3 halfExtents, float distance, bool hitTarget) {
                Origin = origin;
                Direction = direction;
                Rotation = rotation;
                HalfExtents = halfExtents;
                Distance = distance;
                HitTarget = hitTarget;
            }

            /// <summary>
            /// Gets the world-space center point at the end of the cast.
            /// </summary>
            public Vector3 EndCenter => Origin + Direction * Distance;
        }
#endif
    }
}