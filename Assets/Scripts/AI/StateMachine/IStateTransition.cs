namespace HoldMyBeer.AI {
    public interface IStateTransition {
        IAIState TargetState { get; }
        bool CanTransition { get; }
        void OnSourceStateEnter();
        void Tick(float deltaTime);
        void OnSourceStateExit();
    }
}