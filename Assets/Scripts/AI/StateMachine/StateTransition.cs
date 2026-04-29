using System;

namespace HoldMyBeer.AI {
    /// <summary>
    /// Basic implementation of a state transition using a condition delegate.
    /// </summary>
    public sealed class StateTransition : IStateTransition {
        private static readonly string ScriptName = $"[{nameof(StateTransition)}]";

        private readonly Func<bool> condition;

        public IAIState TargetState { get; }

        /// <summary>
        /// Creates a new transition using the given condition and target state.
        /// </summary>
        /// <param name="condition">Delegate used to evaluate whether the transition should occur.</param>
        /// <param name="targetState">State to transition into when the condition is true.</param>
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