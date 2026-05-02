using System;
using System.Collections.Generic;

namespace CocaCopa.StateMachine {
    /// <summary>
    /// Runs a single AI state and evaluates its configured transition rules.
    /// </summary>
    internal sealed class StateMachine {
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
        internal void Init(IState initialState) {
            currentState = initialState ?? throw new NullReferenceException($"{ScriptName} {nameof(currentState)}");
            currentState.Enter();
            EnterAllStateTransitions(currentState);
        }

        /// <summary>
        /// Adds transition rules that can be evaluated while the given state is active.
        /// </summary>
        /// <param name="state">Source state these transitions belong to.</param>
        /// <param name="transitionRules">Transition rules to evaluate for the source state.</param>
        internal void AddTransition(IState state, params IStateTransition[] transitionRules) {
            stateTransitions ??= new Dictionary<string, List<IStateTransition>>();

            for (int i = 0; i < transitionRules.Length; i++) {
                if (!stateTransitions.ContainsKey(state.Id)) { stateTransitions.Add(state.Id, new List<IStateTransition>()); }
                IStateTransition transition = transitionRules[i];
                stateTransitions[state.Id].Add(transition);
            }
        }

        /// <summary>
        /// Removes a specific transition rule from the given source state.
        /// If the source state is currently active, the transition is notified via <see cref="IStateTransition.OnSourceStateExit"/> before removal.
        /// </summary>
        /// <param name="state">The source state that owns the transition.</param>
        /// <param name="transitionToRemove">The transition to remove.</param>
        internal void RemoveTransition(IState state, IStateTransition transitionToRemove) {
            if (state == null) { throw new ArgumentException($"{ScriptName} {nameof(state)}"); }
            if (transitionToRemove == null) { throw new ArgumentException($"{ScriptName} {nameof(transitionToRemove)}"); }

            if (!stateTransitions.TryGetValue(state.Id, out List<IStateTransition> transitions)) { return; }

            bool removingFromCurrentState = currentState != null && currentState.Id == state.Id;

            if (removingFromCurrentState && transitions.Contains(transitionToRemove)) { transitionToRemove.OnSourceStateExit(); }

            transitions.Remove(transitionToRemove);

            if (transitions.Count == 0) { stateTransitions.Remove(state.Id); }
        }

        /// <summary>
        /// Removes all transition rules associated with the given source state.
        /// If the state is currently active, all its transitions are notified via <see cref="IStateTransition.OnSourceStateExit"/> before removal.
        /// </summary>
        /// <param name="state">The source state whose transitions should be removed.</param>
        internal void RemoveAllTransitions(IState state) {
            if (state == null) { throw new ArgumentException($"{ScriptName} {nameof(state)}"); }

            if (!stateTransitions.TryGetValue(state.Id, out List<IStateTransition> transitions)) { return; }

            bool removingFromCurrentState = currentState != null && currentState.Id == state.Id;

            if (removingFromCurrentState) {
                for (int i = 0; i < transitions.Count; i++) { transitions[i].OnSourceStateExit(); }
            }

            stateTransitions.Remove(state.Id);
        }

        /// <summary>
        /// Removes all transitions from the state machine.
        /// Only transitions belonging to the currently active state are notified via <see cref="IStateTransition.OnSourceStateExit"/> before clearing.
        /// </summary>
        internal void RemoveAllTransitions() {
            if (currentState != null && stateTransitions.TryGetValue(currentState.Id, out List<IStateTransition> currentTransitions)) {
                for (int i = 0; i < currentTransitions.Count; i++) { currentTransitions[i].OnSourceStateExit(); }
            }

            stateTransitions.Clear();
        }

        /// <summary>
        /// Updates the current state and evaluates its transition rules.
        /// </summary>
        /// <param name="deltaTime">Elapsed time since the previous update.</param>
        internal void Tick(float deltaTime) {
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