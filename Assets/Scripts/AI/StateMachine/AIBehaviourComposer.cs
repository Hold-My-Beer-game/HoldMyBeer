using UnityEngine;

namespace HoldMyBeer.AI {
    /// <summary>
    /// Base class responsible for composing AI states and wiring them into the brain.
    /// </summary>
    public abstract class AIBehaviourComposer : MonoBehaviour {
        /// <summary>
        /// Creates and configures AI states, then returns the initial setup for the brain.
        /// </summary>
        /// <param name="context">Context containing references and data required by the AI.</param>
        /// <param name="brain">The AI brain that will run the composed states.</param>
        /// <returns>Initial movement and combat states for the AI.</returns>
        public abstract AIStateSetup Compose(AIContext context, AIBrain brain);
    }

    /// <summary>
    /// Holds the initial state configuration for movement and combat state machines.
    /// </summary>
    public readonly struct AIStateSetup {
        /// <summary>
        /// Initial state for the movement state machine.
        /// </summary>
        public readonly IAIState MovementState;

        /// <summary>
        /// Initial state for the combat state machine.
        /// </summary>
        public readonly IAIState CombatState;

        public AIStateSetup(IAIState movementState, IAIState combatState) {
            MovementState = movementState;
            CombatState = combatState;
        }
    }
}