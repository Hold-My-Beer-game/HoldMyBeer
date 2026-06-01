using UnityEngine;
using CocaCopa.StateMachine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class ScreamerHideState : IState {
        internal ScreamerHideState(ScreamerContext context, Vector3 hidePosition, float stopDistance) {
            this.context = context;
            this.hidePosition = hidePosition;
            this.stopDistance = stopDistance;
        }

        private readonly ScreamerContext context;
        private readonly Vector3 hidePosition;
        private readonly float stopDistance;

        public bool OnHidePos { get; private set; }

        public string Id => nameof(ScreamerHideState);

        public void Enter() {
            context.Animator.OnRootMotionDataUpdated += Animator_OnRootMotionDataUpdated;
            Vector3 selfPos = context.Self.position;
            context.Path.CalculatePath(selfPos, hidePosition);
            context.Animator.PlayRun();
            OnHidePos = false;
        }

        public void Tick(float deltaTime) {
            MoveOnPath();
        }

        public void Exit() {
            context.Animator.OnRootMotionDataUpdated -= Animator_OnRootMotionDataUpdated;
        }

        private void Animator_OnRootMotionDataUpdated(RootMotionData data) {
            context.Locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }

        private void MoveOnPath() {
            PathFollowData pathData = context.Path.EvaluatePathProgress(context.Self.position);
            context.Locomotion.SetTargetLookDir(pathData.DirToCorner);

            if (!pathData.IsAtFinalCorner || pathData.DistToCorner > stopDistance) { return; }

            context.AnimatorBase.PlayIdle();
            OnHidePos = true;
        }
    }
}