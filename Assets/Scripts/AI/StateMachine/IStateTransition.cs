namespace HoldMyBeer.AI {
    public interface IStateTransition {
        bool CanTransition();
        IAIState TargetState { get; }
    }
}