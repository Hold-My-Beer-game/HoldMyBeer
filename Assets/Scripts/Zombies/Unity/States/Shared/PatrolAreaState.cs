using System;
using CocaCopa.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class PatrolAreaState : IState {
        internal PatrolAreaState(CommonContext context, MoveMode patrolMode, float timeBeforeMoving, float stopDistance) {
            this.context = context;
            this.patrolMode = patrolMode;
            this.timeBeforeMoving = timeBeforeMoving;
            this.stopDistance = stopDistance;
        }

        private static readonly string ScriptName = $"[{nameof(PatrolAreaState)}]";

        private readonly CommonContext context;
        private readonly MoveMode patrolMode;
        private readonly float timeBeforeMoving;
        private readonly float stopDistance;

        private float moveTimer;
        private Transform currPatrolPoint;

        public string Id => nameof(PatrolAreaState);

        public void Enter() {
            context.AnimatorBase.OnRootMotionDataUpdated += Animator_OnRootMotionUpdated;
            moveTimer = timeBeforeMoving;
            CalculatePatrolPath();
        }

        public void Tick(float deltaTime) {
            if (moveTimer > 0f) {
                moveTimer -= deltaTime;
                return;
            }
            if (!MoveOnPath()) { return; }

            moveTimer = timeBeforeMoving;
            CalculatePatrolPath();
        }

        public void Exit() {
            context.AnimatorBase.OnRootMotionDataUpdated -= Animator_OnRootMotionUpdated;
        }

        private void Animator_OnRootMotionUpdated(RootMotionData data) {
            context.Locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }

        private bool MoveOnPath() {
            switch (patrolMode) {
                case MoveMode.Walk: context.AnimatorBase.PlayWalk(); break;
                case MoveMode.Run: context.AnimatorBase.PlayRun(); break;
                default: throw new ArgumentOutOfRangeException($"{ScriptName} {nameof(patrolMode)}");
            }
            PathFollowData pathData = context.Path.EvaluatePathProgress(context.Self.position);
            context.Locomotion.SetTargetLookDir(pathData.DirToCorner);

            if (!pathData.IsAtFinalCorner || pathData.DistToCorner > stopDistance) { return false; }

            context.AnimatorBase.PlayIdle();
            return true;
        }

        private void CalculatePatrolPath() {
            if (context.PatrolArea.IsOccupied(currPatrolPoint)) { context.PatrolArea.ReturnPatrolPoint(currPatrolPoint); }
            Vector3 selfPos = context.Self.position;
            currPatrolPoint = context.PatrolArea.GetPatrolPoint();
            context.Path.CalculatePath(selfPos, currPatrolPoint.position);
        }
    }
}