using System;
using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class ChaseTargetState : IState {
        internal ChaseTargetState(CommonContext context, MoveMode chaseMode, float pathRefreshInterval, float stopDistance) {
            this.context = context;
            this.chaseMode = chaseMode;
            this.pathRefreshInterval = pathRefreshInterval;
            this.stopDistance = stopDistance;
        }

        private static readonly string ScriptName = $"[{nameof(ChaseTargetState)}]";

        private readonly CommonContext context;
        private readonly MoveMode chaseMode;
        private readonly float pathRefreshInterval;
        private readonly float stopDistance;

        private float pathRefreshTimer;
        private Vector3 lastCornerPos;

        public string Id => nameof(ChaseTargetState);

        public void Enter() {
            context.AnimatorBase.OnRootMotionDataUpdated += Animator_OnRootMotionUpdated;
            pathRefreshTimer = 0f;
            switch (chaseMode) {
                case MoveMode.Walk: context.AnimatorBase.PlayWalk(); break;
                case MoveMode.Run: context.AnimatorBase.PlayRun(); break;
                default: throw new ArgumentOutOfRangeException($"{ScriptName} {nameof(chaseMode)}");
            }
        }

        public void Tick(float deltaTime) {
            RecalculatePath(deltaTime);
            MoveOnPath();
        }

        public void Exit() {
            context.AnimatorBase.OnRootMotionDataUpdated -= Animator_OnRootMotionUpdated;
        }

        private void Animator_OnRootMotionUpdated(RootMotionData data) {
            context.Locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }

        private void RecalculatePath(float deltaTime) {
            pathRefreshTimer -= deltaTime;
            if (pathRefreshTimer > 0f) { return; }
            pathRefreshTimer = pathRefreshInterval;

            context.Path.CalculatePath(context.Self.position, context.Target.Col.transform.position);
        }

        private void MoveOnPath() {
            PathFollowData pathData = context.Path.EvaluatePathProgress(context.Self.position);
            Vector3 cornerPos = pathData.CornerPoints[pathData.CurrCornerIndex];

            if (!lastCornerPos.Equals(cornerPos)) {
                lastCornerPos = cornerPos;
                context.Locomotion.SetTargetLookDir(pathData.DirToCorner);
            }

            if (pathData.IsAtFinalCorner && pathData.DistToCorner <= stopDistance) { context.AnimatorBase.PlayIdle(); }
        }
    }
}