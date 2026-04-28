using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    [RequireComponent(typeof(AIBootstrap_Old))]
    public class AICharacterController : MonoBehaviour {
        [SerializeField] private float pathRefreshInterval = 0.2f;
        [SerializeField] private float stopDistance = 1.5f;

        private static readonly string ScriptName = $"[{nameof(AICharacterController)}]";

        private Transform target;
        private AILocomotion locomotion;
        private AIAnimator animator;
        private AIPath path;

        private bool installed;
        private bool initialized;

        private Vector3 lastCornerPos;
        private float pathRefreshTimer;
        private Vector3[] cornerPoints;

        internal void Install(Transform targetRef, AIPath pathRef) {
            if (installed) { throw new InvalidOperationException($"{ScriptName} Already installed"); }

            target = targetRef;
            path = pathRef;
            animator = GetComponentInChildren<AIAnimator>();
            locomotion = GetComponent<AILocomotion>();

            ValidateState();
            installed = true;
        }

        internal void Init() {
            if (!installed) { throw new InvalidOperationException($"{ScriptName} Called {nameof(Init)} before {nameof(Install)}"); }
            if (initialized) { throw new InvalidOperationException($"{ScriptName} Already initialized"); }

            animator.OnRootMotionDataUpdated += Animator_OnRootMotionDataUpdated;

            initialized = true;
        }

        private void Animator_OnRootMotionDataUpdated(RootMotionData data) {
            locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }

        private void Update() {
            ValidateInstallation();

            RecalculatePath();
            MoveOnPath();
        }

        private void RecalculatePath() {
            pathRefreshTimer -= Time.deltaTime;
            if (pathRefreshTimer > 0f) { return; }
            pathRefreshTimer = pathRefreshInterval;

            path.CalculatePath(transform.position, target.position);
        }

        private void MoveOnPath() {
            PathFollowData pathData = path.EvaluatePathProgress(transform.position);
            Vector3 cornerPos = pathData.CornerPoints[pathData.CurrCornerIndex];
            cornerPoints = pathData.CornerPoints;

            if (!lastCornerPos.Equals(cornerPos)) {
                lastCornerPos = cornerPos;
                locomotion.SetTargetLookDir(pathData.DirToCorner);
                animator.SetTargetLocomotionSpeed(1f);
            }

            if (pathData.IsAtFinalCorner && pathData.DistToCorner <= stopDistance) { animator.SetTargetLocomotionSpeed(0f); }
        }

        private void ValidateState() {
            if (target == null) { throw new NullReferenceException($"{ScriptName} {nameof(target)}"); }
            if (path == null) { throw new NullReferenceException($"{ScriptName} {nameof(path)}"); }
            if (animator == null) { throw new NullReferenceException($"{ScriptName} {nameof(animator)}"); }
            if (locomotion == null) { throw new NullReferenceException($"{ScriptName} {nameof(locomotion)}"); }
        }

        private void ValidateInstallation() {
            if (installed && initialized) { return; }
            enabled = false;
            throw new InvalidOperationException($"{ScriptName} Cannot run Update method: Not initialized and/or installed");
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (cornerPoints is not { Length: > 1 }) { return; }

            for (int i = 0; i < cornerPoints.Length; i++) {
                Vector3 corner = cornerPoints[i];
                Vector3? nextCorner = i + 1 < cornerPoints.Length
                    ? cornerPoints[i + 1]
                    : null;

                Gizmos.color = Color.red;
                Gizmos.DrawSphere(corner, 0.1f);

                if (!nextCorner.HasValue) { return; }

                Gizmos.color = Color.green;
                Gizmos.DrawLine(corner, nextCorner.Value);
            }
        }
#endif
    }
}