using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    public class SearchTargetState : IState {
        public SearchTargetState(ZombieContext context, float stopDistance) {
            this.context = context;
            this.stopDistance = stopDistance;
        }

        private readonly ZombieContext context;
        private readonly float stopDistance;

        private Vector3 lastCornerPos;

        public string Id => nameof(SearchTargetState);
        public bool OnLastKnownPos { get; private set; }

        public void Enter() {
            Vector3 selfPos = context.Self.position;
            Vector3 targetPos = context.Target.Col.transform.position;
            context.Path.CalculatePath(selfPos, targetPos);
            context.Animator.SetTargetLocomotionSpeed(1f);
            context.Animator.OnRootMotionDataUpdated += Animator_OnRootMotionUpdated;
        }

        public void Tick(float deltaTime) {
            MoveOnPath();
        }

        public void Exit() {
            context.Animator.OnRootMotionDataUpdated -= Animator_OnRootMotionUpdated;
            OnLastKnownPos = false;
        }

        private void Animator_OnRootMotionUpdated(RootMotionData data) {
            context.Locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }

        private void MoveOnPath() {
            PathFollowData pathData = context.Path.EvaluatePathProgress(context.Self.position);
            context.Locomotion.SetTargetLookDir(pathData.DirToCorner);

            if (!pathData.IsAtFinalCorner || pathData.DistToCorner > stopDistance) { return; }

            context.Animator.SetTargetLocomotionSpeed(0f);
            OnLastKnownPos = true;
        }
    }
}