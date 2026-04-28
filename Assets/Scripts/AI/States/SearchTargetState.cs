using UnityEngine;

namespace HoldMyBeer.AI {
    public class SearchTargetState : IAIState {
        public SearchTargetState(AIContext context, float stopDistance) {
            this.context = context;
            this.stopDistance = stopDistance;
        }

        private readonly AIContext context;
        private readonly float stopDistance;

        private Vector3 lastCornerPos;

        public string Id => nameof(SearchTargetState);
        public bool OnLastKnownPos { get; private set; }

        public void Enter() {
            Vector3 selfPos = context.Self.position;
            Vector3 targetPos = context.Target.position;
            context.Path.CalculatePath(selfPos, targetPos);
            context.Animator.SetTargetLocomotionSpeed(1f);
            context.Animator.OnRootMotionDataUpdated += Animator_OnRootMotionUpdated;
        }

        public void Tick(float deltaTime) {
            MoveOnPath();
        }

        public void Exit() {
            OnLastKnownPos = false;
            context.Animator.OnRootMotionDataUpdated -= Animator_OnRootMotionUpdated;
        }

        private void Animator_OnRootMotionUpdated(RootMotionData data) {
            context.Self.position += data.DeltaPosition;
        }

        private void MoveOnPath() {
            PathFollowData pathData = context.Path.EvaluatePathProgress(context.Self.position);
            Vector3 cornerPos = pathData.CornerPoints[pathData.CurrCornerIndex];

            if (!lastCornerPos.Equals(cornerPos)) {
                lastCornerPos = cornerPos;
                context.Locomotion.SetTargetLookDir(pathData.DirToCorner);
            }

            if (pathData.IsAtFinalCorner && pathData.DistToCorner <= stopDistance) {
                OnLastKnownPos = true;
                context.Animator.SetTargetLocomotionSpeed(0f);
            }
        }
    }
}