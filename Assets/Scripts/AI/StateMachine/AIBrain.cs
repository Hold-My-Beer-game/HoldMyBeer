using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    public sealed class AIBrain : MonoBehaviour {
        private static readonly string ScriptName = $"[{nameof(AIBrain)}]";

        private readonly AIStateMachine movementMachine = new();
        private readonly AIStateMachine combatMachine = new();

        private bool initialized;

        internal void Init(IAIState initialMovementState, IAIState initialCombatState) {
            if (initialized) { throw new InvalidOperationException($"{ScriptName} Already initialized"); }

            if (initialMovementState != null) { movementMachine.Init(initialMovementState); }
            if (initialCombatState != null) { combatMachine.Init(initialCombatState); }

            initialized = true;
        }

        public void AddMovementTransition(IAIState fromState, params IStateTransition[] transitions) {
            movementMachine.AddTransition(fromState, transitions);
        }

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