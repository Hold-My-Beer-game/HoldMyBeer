using CocaCopa.StateMachine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class IdleMovementState : IState {
        internal IdleMovementState(CommonContext context) {
            this.context = context;
        }

        private readonly CommonContext context;

        public string Id => nameof(IdleMovementState);

        public void Enter() {
            context.AnimatorBase.PlayIdle();
        }

        public void Tick(float deltaTime) { }

        public void Exit() { }
    }
}