namespace CocaCopa.StateMachine {
    public interface IStateMachineBrain {
        string CurrentMovementStateID { get; }
        string CurrentCombatStateID { get; }
        void AddMovementTransition(IState fromState, params IStateTransition[] transitions);
        void AddCombatTransition(IState fromState, params IStateTransition[] transitions);
    }
}