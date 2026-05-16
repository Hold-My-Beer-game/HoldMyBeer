using CocaCopa.StateMachine;

namespace HoldMyBeer.Zombies.Unity {
    public class IdleCombatState : IState {
        public string Id => nameof(IdleCombatState);
        public void Enter() { }
        public void Tick(float deltaTime) { }
        public void Exit() { }
    }
}