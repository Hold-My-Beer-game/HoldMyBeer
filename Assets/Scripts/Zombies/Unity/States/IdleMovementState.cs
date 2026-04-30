using CocaCopa.StateMachine;

namespace HoldMyBeer.Zombies.Unity {
    public sealed class IdleMovementState : IState {
        public IdleMovementState(ZombieContext context) {
            this.context = context;
        }

        private readonly ZombieContext context;

        public string Id => nameof(IdleMovementState);

        public void Enter() {
            context.Animator.SetTargetLocomotionSpeed(0f);
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}