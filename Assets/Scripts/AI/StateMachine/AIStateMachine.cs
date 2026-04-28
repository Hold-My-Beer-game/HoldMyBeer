using System;
using System.Collections.Generic;

namespace HoldMyBeer.AI {
    public sealed class AIStateMachine {
        private static readonly string ScriptName = $"[{nameof(AIStateMachine)}]";
        private Dictionary<string, List<IStateTransition>> stateTransitions = new();

        private IAIState currentState;

        public void Init(IAIState initialState) {
            currentState = initialState ?? throw new NullReferenceException($"{ScriptName} {nameof(currentState)}");
            currentState?.Enter();
        }

        public void AddTransition(IAIState state, params IStateTransition[] transitionRules) {
            stateTransitions ??= new Dictionary<string, List<IStateTransition>>();

            for (int i = 0; i < transitionRules.Length; i++) {
                if (!stateTransitions.ContainsKey(state.Id)) { stateTransitions.Add(state.Id, new List<IStateTransition>()); }
                IStateTransition transition = transitionRules[i];
                stateTransitions[state.Id].Add(transition);
            }
        }

        public void Tick(float deltaTime) {
            if (currentState == null) { return; }

            currentState.Tick(deltaTime);

            if (!stateTransitions.TryGetValue(currentState.Id, out List<IStateTransition> transitions)) { return; }

            for (int i = 0; i < transitions.Count; i++) {
                IStateTransition transition = transitions[i];
                if (!transition.CanTransition()) { continue; }
                SwitchState(transition.TargetState);
                break;
            }
        }

        private void SwitchState(IAIState nextState) {
            currentState.Exit();
            currentState = nextState;
            currentState.Enter();
        }
    }
}