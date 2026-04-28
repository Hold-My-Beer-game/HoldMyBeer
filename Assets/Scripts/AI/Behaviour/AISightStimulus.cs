using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldMyBeer.AI {
    public class AISightStimulus : MonoBehaviour {
        [Header("Origin")]
        [SerializeField] private Vector3 originOffset = Vector3.zero;

        [Header("Sight")]
        [SerializeField] private float sightDistance = 10f;
        [SerializeField] private float leftAngle = 45f;
        [SerializeField] private float rightAngle = 45f;
        [SerializeField] private float upAngle = 30f;
        [SerializeField] private float downAngle = 30f;

        [Header("Sight Box")]
        [SerializeField] private float sightBoxDepth = 2f;
        [SerializeField] [Min(1)] private int horizontalCount = 1;
        [SerializeField] [Min(1)] private int verticalCount = 1;
        [SerializeField] [Range(1, 100)] private float sightSize = 100f;

        [Header("Raycast")]
        [SerializeField] private LayerMask sightMask = ~0;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        public Vector3 Origin => transform.TransformPoint(originOffset);

#if UNITY_EDITOR
        public float LeftAngle => leftAngle;
        public float RightAngle => rightAngle;
        public float UpAngle => upAngle;
        public float DownAngle => downAngle;

        private readonly List<SightBoxCastDebugData> debugBoxCasts = new();
        public IReadOnlyList<SightBoxCastDebugData> DebugBoxCasts => debugBoxCasts;
#endif

        public bool CanSee(Collider targetCollider) {
#if UNITY_EDITOR
            debugBoxCasts.Clear();
#endif
            bool success = false;
            Vector3 toTarget = targetCollider.ClosestPoint(Origin) - Origin;
            if (toTarget.sqrMagnitude > sightDistance * sightDistance) { return false; }

            Vector3 relativeRight = GetRelativeRight(transform, targetCollider.transform);
            Vector3 relativeUp = targetCollider.transform.up;
            Vector3 targetSize = targetCollider.bounds.size;

            float horizontalStep = targetSize.x / horizontalCount;
            float verticalStep = targetSize.y / verticalCount;

            Vector3 size = new(horizontalStep * sightSize / 100f, verticalStep * sightSize / 100f, sightBoxDepth);
            Vector3 startPos = targetCollider.bounds.center;
            startPos += relativeRight * (horizontalStep / 2f - targetSize.x / 2f);
            startPos += relativeUp * (targetSize.y / 2f - verticalStep / 2f);

            for (int i = 0; i < verticalCount; i++) {
                Vector3 rowStart = startPos - i * verticalStep * relativeUp;
                for (int j = 0; j < horizontalCount; j++) {
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

        private static Vector3 GetRelativeRight(Transform from, Transform to) {
            Vector3 targetPos = to.position;
            targetPos.y = 0f;
            Vector3 myPos = from.position;
            myPos.y = 0f;
            Vector3 toTargetDir = (targetPos - myPos).normalized;

            return Vector3.Cross(from.up, toTargetDir);
        }

        private Quaternion GetSightBoxRotation(Vector3 direction) {
            if (direction.sqrMagnitude <= 0.0001f) { return transform.rotation; }

            return Quaternion.LookRotation(direction, transform.up);
        }

#if UNITY_EDITOR
        public Vector3 GetConeEndPoint(float yaw, float pitch) {
            Quaternion rotation = transform.rotation * Quaternion.Euler(-pitch, yaw, 0f);
            Vector3 direction = rotation * Vector3.forward;

            return Origin + direction * sightDistance;
        }

        public readonly struct SightBoxCastDebugData {
            public readonly Vector3 Origin;
            public readonly Vector3 Direction;
            public readonly Quaternion Rotation;
            public readonly Vector3 HalfExtents;
            public readonly float Distance;
            public readonly bool HitTarget;

            public SightBoxCastDebugData(Vector3 origin, Vector3 direction, Quaternion rotation, Vector3 halfExtents, float distance, bool hitTarget) {
                Origin = origin;
                Direction = direction;
                Rotation = rotation;
                HalfExtents = halfExtents;
                Distance = distance;
                HitTarget = hitTarget;
            }

            public Vector3 EndCenter => Origin + Direction * Distance;
        }
#endif
    }
}