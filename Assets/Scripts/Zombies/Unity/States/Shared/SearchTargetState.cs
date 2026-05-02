using System;
using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class SearchTargetState : IState {
        internal SearchTargetState(CommonContext context, MoveMode searchMode, float stopDistance) {
            this.context = context;
            this.searchMode = searchMode;
            this.stopDistance = stopDistance;
        }

        private static readonly string ScriptName = $"[{nameof(SearchTargetState)}]";

        private readonly CommonContext context;
        private readonly MoveMode searchMode;
        private readonly float stopDistance;

        private Vector3 lastCornerPos;

        public string Id => nameof(SearchTargetState);
        public bool OnLastKnownPos { get; private set; }

        public void Enter() {
            Vector3 selfPos = context.Self.position;
            Vector3 targetPos = context.Target.Col.transform.position;
            context.Path.CalculatePath(selfPos, targetPos);
            switch (searchMode) {
                case MoveMode.Walk: context.AnimatorBase.PlayWalk(); break;
                case MoveMode.Run: context.AnimatorBase.PlayRun(); break;
                default: throw new ArgumentOutOfRangeException($"{ScriptName} {nameof(searchMode)}");
            }
            context.AnimatorBase.OnRootMotionDataUpdated += Animator_OnRootMotionUpdated;
        }

        public void Tick(float deltaTime) {
            MoveOnPath();
        }

        public void Exit() {
            context.AnimatorBase.OnRootMotionDataUpdated -= Animator_OnRootMotionUpdated;
            OnLastKnownPos = false;
        }

        private void Animator_OnRootMotionUpdated(RootMotionData data) {
            context.Locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }

        private void MoveOnPath() {
            PathFollowData pathData = context.Path.EvaluatePathProgress(context.Self.position);
            context.Locomotion.SetTargetLookDir(pathData.DirToCorner);

            if (!pathData.IsAtFinalCorner || pathData.DistToCorner > stopDistance) { return; }

            context.AnimatorBase.PlayIdle();
            OnLastKnownPos = true;
        }
    }
}