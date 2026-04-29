namespace HoldMyBeer.AI {
    /// <summary>
    /// Defines a transition rule between AI states.
    /// Responsible for evaluating conditions and signaling when a state change should occur.
    /// </summary>
    public interface IStateTransition {
        /// <summary>
        /// Target state that will be entered if this transition is triggered.
        /// </summary>
        IAIState TargetState { get; }

        /// <summary>
        /// Indicates whether the transition conditions have been met.
        /// </summary>
        bool CanTransition { get; }

        /// <summary>
        /// Called when the source state becomes active.<br/>
        /// Used to initialize or reset transition state.
        /// </summary>
        void OnSourceStateEnter();

        /// <summary>
        /// Updates the transition logic.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last update.</param>
        void Tick(float deltaTime);

        /// <summary>
        /// Called when the source state is exited.<br/>
        /// Used to clean up or reset any state.
        /// </summary>
        void OnSourceStateExit();
    }
}