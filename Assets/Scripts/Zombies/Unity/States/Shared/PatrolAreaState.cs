using System;
using CocaCopa.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class PatrolAreaState : IState {
        internal PatrolAreaState(CommonContext context, Vector3[] patrolPoints, MoveMode patrolMode, float timeBeforeMoving, float stopDistance) {
            this.context = context;
            this.patrolPoints = patrolPoints;
            this.patrolMode = patrolMode;
            this.timeBeforeMoving = timeBeforeMoving;
            this.stopDistance = stopDistance;
        }

        private static readonly string ScriptName = $"[{nameof(PatrolAreaState)}]";

        private readonly CommonContext context;
        private readonly Vector3[] patrolPoints;
        private readonly MoveMode patrolMode;
        private readonly float timeBeforeMoving;
        private readonly float stopDistance;

        private float moveTimer;
        private int currentPatrolIndex;

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
            CalculatePatrolPath(currentPatrolIndex);
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

        private void CalculatePatrolPath(int excludePatrolIndex = -1) {
            currentPatrolIndex = GetRandomPatrolIndex(patrolPoints, excludePatrolIndex);
            Vector3 selfPos = context.Self.position;
            Vector3 patrolPos = patrolPoints[currentPatrolIndex];
            context.Path.CalculatePath(selfPos, patrolPos);
        }

        private static int GetRandomPatrolIndex(Vector3[] points, int excludeIndex = -1) {
            int random = Random.Range(0, points.Length);
            if (excludeIndex >= 0 && random == excludeIndex) {
                random++;
                if (random >= points.Length) { random = 0; }
            }
            return random;
        }
    }
}