using UnityEngine;

namespace CocaCopa.StateMachine {
    /// <summary>
    /// Coordinates the AI state machines responsible for movement and combat behavior.
    /// </summary>
    public sealed class StateMachineBrain : MonoBehaviour, IStateMachineBrain {
        private static readonly string ScriptName = $"[{nameof(StateMachineBrain)}]";

        private readonly StateMachine movementMachine = new();
        private readonly StateMachine combatMachine = new();

        public string CurrentMovementStateID => movementMachine.StateID;
        public string CurrentCombatStateID => combatMachine.StateID;

        private void Start() {
            var composer = GetComponent<IStateMachineComposer>();
            StateSetup setup = composer.Compose(this);
            Init(setup.MovementState, setup.CombatState);
        }

        private void Update() {
            movementMachine.Tick(Time.deltaTime);
            combatMachine.Tick(Time.deltaTime);
        }

        public void ForceMovementState(IState state) {
            movementMachine.ForceState(state);
        }

        public void ForceCombatState(IState state) {
            combatMachine.ForceState(state);
        }

        private void Init(IState initialMovementState, IState initialCombatState) {
            if (initialMovementState != null) { movementMachine.Init(initialMovementState); }
            if (initialCombatState != null) { combatMachine.Init(initialCombatState); }
        }

        public void AddMovementTransition(IState fromState, params IStateTransition[] transitions) {
            movementMachine.AddTransition(fromState, transitions);
        }

        public void AddCombatTransition(IState fromState, params IStateTransition[] transitions) {
            combatMachine.AddTransition(fromState, transitions);
        }

        public void RemoveMovementTransition(IState state, IStateTransition transition) {
            movementMachine.RemoveTransition(state, transition);
        }

        public void RemoveMovementTransitions(IState state) {
            movementMachine.RemoveAllTransitions(state);
        }

        public void RemoveAllMovementTransitions() {
            movementMachine.RemoveAllTransitions();
        }

        public void RemoveCombatTransition(IState state, IStateTransition transition) {
            combatMachine.RemoveTransition(state, transition);
        }

        public void RemoveCombatTransitions(IState state) {
            combatMachine.RemoveAllTransitions(state);
        }

        public void RemoveAllCombatTransitions() {
            combatMachine.RemoveAllTransitions();
        }

        public void RemoveAllTransitions() {
            movementMachine.RemoveAllTransitions();
            combatMachine.RemoveAllTransitions();
        }
    }
}