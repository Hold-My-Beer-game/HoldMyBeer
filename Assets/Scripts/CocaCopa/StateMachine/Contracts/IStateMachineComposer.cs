using UnityEngine;

namespace CocaCopa.StateMachine {
    /// <summary>
    /// Base class responsible for composing AI states and wiring them into the brain.
    /// </summary>
    public interface IStateMachineComposer {
        /// <summary>
        /// Creates and configures AI states, then returns the initial setup for the brain.
        /// </summary>
        /// <param name="brain">The AI brain that will run the composed states.</param>
        /// <returns>Initial movement and combat states for the AI.</returns>
        StateSetup Compose(IStateMachineBrain brain);
    }
}