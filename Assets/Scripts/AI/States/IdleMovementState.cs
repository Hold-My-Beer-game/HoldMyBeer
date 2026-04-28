namespace HoldMyBeer.AI {
    public sealed class IdleMovementState : IAIState {
        private readonly AIContext context;

        public IdleMovementState(AIContext context) {
            this.context = context;
        }

        public string Id => nameof(IdleMovementState);

        public void Enter() {
            context.Animator.SetTargetLocomotionSpeed(0f);
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}