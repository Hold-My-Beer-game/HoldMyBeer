using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    /// <summary>
    /// Coordinates the AI state machines responsible for movement and combat behavior.
    /// </summary>
    public sealed class AIBrain : MonoBehaviour {
        private static readonly string ScriptName = $"[{nameof(AIBrain)}]";

        private readonly AIStateMachine movementMachine = new();
        private readonly AIStateMachine combatMachine = new();

        private bool initialized;

        /// <summary>
        /// Initializes the AI brain with optional starting states for movement and combat.
        /// </summary>
        internal void Init(IAIState initialMovementState, IAIState initialCombatState) {
            if (initialized) { throw new InvalidOperationException($"{ScriptName} Already initialized"); }

            if (initialMovementState != null) { movementMachine.Init(initialMovementState); }
            if (initialCombatState != null) { combatMachine.Init(initialCombatState); }

            initialized = true;
        }

        /// <summary>
        /// Adds transition rules to the movement state machine.
        /// </summary>
        /// <param name="fromState">Source movement state.</param>
        /// <param name="transitions">Transitions evaluated while the source state is active.</param>
        public void AddMovementTransition(IAIState fromState, params IStateTransition[] transitions) {
            movementMachine.AddTransition(fromState, transitions);
        }

        /// <summary>
        /// Adds transition rules to the combat state machine.
        /// </summary>
        /// <param name="fromState">Source combat state.</param>
        /// <param name="transitions">Transitions evaluated while the source state is active.</param>
        public void AddCombatTransition(IAIState fromState, params IStateTransition[] transitions) {
            combatMachine.AddTransition(fromState, transitions);
        }

        private void Update() {
            if (!initialized) {
                enabled = false;
                throw new InvalidOperationException($"{ScriptName} Script Disabled: Not initialized");
            }

            movementMachine.Tick(Time.deltaTime);
            combatMachine.Tick(Time.deltaTime);
        }
    }
}