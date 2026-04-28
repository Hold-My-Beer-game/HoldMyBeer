using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    public sealed class StateTransition : IStateTransition {
        private static readonly string ScriptName = $"[{nameof(StateTransition)}]";

        private readonly Func<bool> condition;

        public IAIState TargetState { get; }

        public StateTransition(Func<bool> condition, IAIState targetState) {
            this.condition = condition ?? throw new ArgumentNullException($"{ScriptName} {nameof(condition)}");
            TargetState = targetState ?? throw new ArgumentNullException($"{ScriptName} {nameof(targetState)}");
        }

        public bool CanTransition => condition();
        public void OnSourceStateEnter() { }
        public void Tick(float deltaTime) { }
        public void OnSourceStateExit() { }
    }
}