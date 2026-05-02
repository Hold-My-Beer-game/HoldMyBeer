using CocaCopa.StateMachine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class ScreamerHideState : IState {
        internal ScreamerHideState(ScreamerContext context) {
            this.context = context;
        }

        private readonly ScreamerContext context;

        public string Id => nameof(ScreamerHideState);

        public void Enter() {
            context.Animator.OnRootMotionDataUpdated += Animator_OnRootMotionDataUpdated;
        }

        public void Tick(float deltaTime) {
            //
        }

        public void Exit() {
            context.Animator.OnRootMotionDataUpdated -= Animator_OnRootMotionDataUpdated;
        }

        private void Animator_OnRootMotionDataUpdated(RootMotionData data) {
            context.Locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }
    }
}