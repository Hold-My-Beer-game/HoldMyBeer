namespace HoldMyBeer.AI {
    public class IdleCombatState : IAIState {
        public string Id => nameof(IdleCombatState);
        public void Enter() { }
        public void Tick(float deltaTime) { }
        public void Exit() { }
    }
}