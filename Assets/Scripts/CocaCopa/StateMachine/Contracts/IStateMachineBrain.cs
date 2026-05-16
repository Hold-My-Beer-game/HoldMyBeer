namespace CocaCopa.StateMachine {
    /// <summary>
    /// Provides consumer-facing control over the layered AI state machines.
    /// </summary>
    public interface IStateMachineBrain {
        /// <summary>
        /// Gets the ID of the currently active movement state.
        /// </summary>
        string CurrentMovementStateID { get; }

        /// <summary>
        /// Gets the ID of the currently active combat state.
        /// </summary>
        string CurrentCombatStateID { get; }

        /// <summary>
        /// Forces the movement state machine to immediately switch to the given state.
        /// </summary>
        /// <param name="state">Movement state to switch into.</param>
        void ForceMovementState(IState state);

        /// <summary>
        /// Forces the combat state machine to immediately switch to the given state.
        /// </summary>
        /// <param name="state">Combat state to switch into.</param>
        void ForceCombatState(IState state);

        /// <summary>
        /// Adds transition rules that can be evaluated while the given movement state is active.
        /// </summary>
        /// <param name="fromState">Source movement state these transitions belong to.</param>
        /// <param name="transitions">Transition rules to evaluate for the source movement state.</param>
        void AddMovementTransition(IState fromState, params IStateTransition[] transitions);

        /// <summary>
        /// Adds transition rules that can be evaluated while the given combat state is active.
        /// </summary>
        /// <param name="fromState">Source combat state these transitions belong to.</param>
        /// <param name="transitions">Transition rules to evaluate for the source combat state.</param>
        void AddCombatTransition(IState fromState, params IStateTransition[] transitions);

        /// <summary>
        /// Removes a specific transition rule from the given movement source state.
        /// If the source state is currently active, the transition is notified before removal.
        /// </summary>
        /// <param name="state">The movement source state that owns the transition.</param>
        /// <param name="transition">The transition to remove.</param>
        void RemoveMovementTransition(IState state, IStateTransition transition);

        /// <summary>
        /// Removes all transition rules associated with the given movement source state.
        /// If the source state is currently active, its transitions are notified before removal.
        /// </summary>
        /// <param name="state">The movement source state whose transitions should be removed.</param>
        void RemoveMovementTransitions(IState state);

        /// <summary>
        /// Removes all transition rules from the movement state machine.
        /// Only transitions belonging to the currently active movement state are notified before clearing.
        /// </summary>
        void RemoveAllMovementTransitions();

        /// <summary>
        /// Removes a specific transition rule from the given combat source state.
        /// If the source state is currently active, the transition is notified before removal.
        /// </summary>
        /// <param name="state">The combat source state that owns the transition.</param>
        /// <param name="transition">The transition to remove.</param>
        void RemoveCombatTransition(IState state, IStateTransition transition);

        /// <summary>
        /// Removes all transition rules associated with the given combat source state.
        /// If the source state is currently active, its transitions are notified before removal.
        /// </summary>
        /// <param name="state">The combat source state whose transitions should be removed.</param>
        void RemoveCombatTransitions(IState state);

        /// <summary>
        /// Removes all transition rules from the combat state machine.
        /// Only transitions belonging to the currently active combat state are notified before clearing.
        /// </summary>
        void RemoveAllCombatTransitions();

        /// <summary>
        /// Removes all transition rules from both the movement and combat state machines.
        /// Only transitions belonging to the currently active states are notified before clearing.
        /// </summary>
        void RemoveAllTransitions();
    }
}