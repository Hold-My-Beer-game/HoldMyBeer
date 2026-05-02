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

        public void ForceMovementState(IState state) {
            movementMachine.ForceState(state);
        }

        public void ForceCombatState(IState state) {
            combatMachine.ForceState(state);
        }

        private void Start() {
            var composer = GetComponent<IStateMachineComposer>();
            StateSetup setup = composer.Compose(this);
            Init(setup.MovementState, setup.CombatState);
        }

        /// <summary>
        /// Initializes the AI brain with optional starting states for movement and combat.
        /// </summary>
        private void Init(IState initialMovementState, IState initialCombatState) {
            if (initialMovementState != null) { movementMachine.Init(initialMovementState); }
            if (initialCombatState != null) { combatMachine.Init(initialCombatState); }
        }

        /// <summary>
        /// Adds transition rules to the movement state machine.
        /// </summary>
        /// <param name="fromState">Source movement state.</param>
        /// <param name="transitions">Transitions evaluated while the source state is active.</param>
        public void AddMovementTransition(IState fromState, params IStateTransition[] transitions) {
            movementMachine.AddTransition(fromState, transitions);
        }

        /// <summary>
        /// Adds transition rules to the combat state machine.
        /// </summary>
        /// <param name="fromState">Source combat state.</param>
        /// <param name="transitions">Transitions evaluated while the source state is active.</param>
        public void AddCombatTransition(IState fromState, params IStateTransition[] transitions) {
            combatMachine.AddTransition(fromState, transitions);
        }

        private void Update() {
            movementMachine.Tick(Time.deltaTime);
            combatMachine.Tick(Time.deltaTime);
        }
    }
}