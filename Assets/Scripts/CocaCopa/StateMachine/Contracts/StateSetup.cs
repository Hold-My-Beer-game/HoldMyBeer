namespace CocaCopa.StateMachine {
    /// <summary>
    /// Holds the initial state configuration for movement and combat state machines.
    /// </summary>
    public readonly struct StateSetup {
        /// <summary>
        /// Initial state for the movement state machine.
        /// </summary>
        public readonly IState MovementState;

        /// <summary>
        /// Initial state for the combat state machine.
        /// </summary>
        public readonly IState CombatState;

        public StateSetup(IState movementState, IState combatState) {
            MovementState = movementState;
            CombatState = combatState;
        }
    }
}