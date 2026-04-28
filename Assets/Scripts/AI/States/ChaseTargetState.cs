using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    public sealed class ChaseTargetState : IAIState {
        private readonly AIContext context;
        private readonly float pathRefreshInterval;
        private readonly float stopDistance;

        private float pathRefreshTimer;
        private Vector3 lastCornerPos;

        public ChaseTargetState(AIContext context, float pathRefreshInterval, float stopDistance) {
            this.context = context;
            this.pathRefreshInterval = pathRefreshInterval;
            this.stopDistance = stopDistance;
        }

        public string Id => nameof(ChaseTargetState);

        public void Enter() {
            pathRefreshTimer = 0f;
            context.Animator.SetTargetLocomotionSpeed(1f);
            context.Animator.OnRootMotionDataUpdated += Animator_OnRootMotionUpdated;
        }

        private void Animator_OnRootMotionUpdated(RootMotionData data) {
            context.Self.position += data.DeltaPosition;
        }

        public void Tick(float deltaTime) {
            RecalculatePath(deltaTime);
            MoveOnPath();
        }

        public void Exit() {
            context.Animator.SetTargetLocomotionSpeed(0f);
            context.Animator.OnRootMotionDataUpdated -= Animator_OnRootMotionUpdated;
        }

        private void RecalculatePath(float deltaTime) {
            pathRefreshTimer -= deltaTime;
            if (pathRefreshTimer > 0f) { return; }
            pathRefreshTimer = pathRefreshInterval;

            context.Path.CalculatePath(context.Self.position, context.Target.position);
        }

        private void MoveOnPath() {
            PathFollowData pathData = context.Path.EvaluatePathProgress(context.Self.position);
            Vector3 cornerPos = pathData.CornerPoints[pathData.CurrCornerIndex];

            if (!lastCornerPos.Equals(cornerPos)) {
                lastCornerPos = cornerPos;
                context.Locomotion.SetTargetLookDir(pathData.DirToCorner);
                context.Animator.SetTargetLocomotionSpeed(1f);
            }

            if (pathData.IsAtFinalCorner && pathData.DistToCorner <= stopDistance) { context.Animator.SetTargetLocomotionSpeed(0f); }
        }
    }
}