using System;
using System.Collections.Generic;

namespace CocaCopa.StateMachine {
    /// <summary>
    /// Runs a single AI state and evaluates its configured transition rules.
    /// </summary>
    public sealed class StateMachine {
        private static readonly string ScriptName = $"[{nameof(StateMachine)}]";
        private Dictionary<string, List<IStateTransition>> stateTransitions = new();

        private IState currentState;

        internal string StateID => currentState?.Id ?? "Null";

        internal void ForceState(IState stateId) {
            SwitchState(stateId);
        }

        /// <summary>
        /// Initializes the state machine with the first active state.
        /// </summary>
        /// <param name="initialState">State to enter when the state machine starts.</param>
        public void Init(IState initialState) {
            currentState = initialState ?? throw new NullReferenceException($"{ScriptName} {nameof(currentState)}");
            currentState.Enter();
            EnterAllStateTransitions(currentState);
        }

        /// <summary>
        /// Adds transition rules that can be evaluated while the given state is active.
        /// </summary>
        /// <param name="state">Source state these transitions belong to.</param>
        /// <param name="transitionRules">Transition rules to evaluate for the source state.</param>
        public void AddTransition(IState state, params IStateTransition[] transitionRules) {
            stateTransitions ??= new Dictionary<string, List<IStateTransition>>();

            for (int i = 0; i < transitionRules.Length; i++) {
                if (!stateTransitions.ContainsKey(state.Id)) { stateTransitions.Add(state.Id, new List<IStateTransition>()); }
                IStateTransition transition = transitionRules[i];
                stateTransitions[state.Id].Add(transition);
            }
        }

        /// <summary>
        /// Updates the current state and evaluates its transition rules.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since the previous update.</param>
        public void Tick(float deltaTime) {
            if (currentState == null) { return; }

            currentState.Tick(deltaTime);

            if (!stateTransitions.TryGetValue(currentState.Id, out List<IStateTransition> transitions)) { return; }

            for (int i = 0; i < transitions.Count; i++) {
                IStateTransition transition = transitions[i];
                transition.Tick(deltaTime);

                if (!transition.CanTransition) { continue; }

                SwitchState(transition.TargetState);
                break;
            }
        }

        /// <summary>
        /// Exits the current state and enters the next state.
        /// </summary>
        /// <param name="nextState">State to switch into.</param>
        private void SwitchState(IState nextState) {
            ExitAllStateTransitions(currentState);
            currentState.Exit();
            currentState = nextState;
            currentState.Enter();
            EnterAllStateTransitions(currentState);
        }

        /// <summary>
        /// Notifies all transitions assigned to the given state that their source state has exited.
        /// </summary>
        /// <param name="state">Source state whose transitions should be notified.</param>
        private void ExitAllStateTransitions(IState state) {
            if (state == null) { throw new ArgumentException($"{ScriptName} {nameof(state)}"); }
            if (!stateTransitions.TryGetValue(state.Id, out List<IStateTransition> transitions)) { return; }

            for (int i = 0; i < transitions.Count; i++) {
                IStateTransition transition = transitions[i];
                transition.OnSourceStateExit();
            }
        }

        /// <summary>
        /// Notifies all transitions assigned to the given state that their source state has entered.
        /// </summary>
        /// <param name="state">Source state whose transitions should be notified.</param>
        private void EnterAllStateTransitions(IState state) {
            if (state == null) { throw new ArgumentException($"{ScriptName} {nameof(state)}"); }
            if (!stateTransitions.TryGetValue(state.Id, out List<IStateTransition> transitions)) { return; }

            for (int i = 0; i < transitions.Count; i++) {
                IStateTransition transition = transitions[i];
                transition.OnSourceStateEnter();
            }
        }
    }
}