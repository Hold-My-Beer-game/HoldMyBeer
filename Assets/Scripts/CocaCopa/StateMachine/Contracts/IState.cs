namespace CocaCopa.StateMachine {
    /// <summary>
    /// Defines a single AI state with lifecycle methods for entering, updating, and exiting.
    /// </summary>
    public interface IState {
        /// <summary>
        /// Unique identifier used by the state machine to track and match transitions.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Called when the state becomes active.
        /// Used for initialization logic.
        /// </summary>
        void Enter();

        /// <summary>
        /// Called every frame while the state is active.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since last update.</param>
        void Tick(float deltaTime);

        /// <summary>
        /// Called when the state is exited.
        /// Used for cleanup logic.
        /// </summary>
        void Exit();
    }
}