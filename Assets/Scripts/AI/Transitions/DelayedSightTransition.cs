using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    public sealed class DelayedSightTransition : IStateTransition {
        private static readonly string ScriptName = $"[{nameof(DelayedSightTransition)}]";

        private readonly AISightStimulus sight;
        private readonly Collider targetCol;
        private readonly float requiredSightTime;

        private float remainingSightTime;

        public bool CanTransition { get; private set; }
        public IAIState TargetState { get; }

        public DelayedSightTransition(AISightStimulus sight, Collider targetCol, float requiredSightTime, IAIState targetState) {
            this.sight = sight ?? throw new ArgumentNullException($"{ScriptName} {nameof(sight)}");
            this.targetCol = targetCol ?? throw new ArgumentNullException($"{ScriptName} {nameof(targetCol)}");
            this.requiredSightTime = Mathf.Max(0f, requiredSightTime);

            TargetState = targetState ?? throw new ArgumentNullException($"{ScriptName} {nameof(targetState)}");

            remainingSightTime = this.requiredSightTime;
            CanTransition = false;
        }

        public void OnSourceStateEnter() { }

        public void Tick(float deltaTime) {
            CanTransition = AllowTransition(deltaTime);
        }

        public void OnSourceStateExit() {
            CanTransition = false;
            remainingSightTime = requiredSightTime;
        }

        private bool AllowTransition(float deltaTime) {
            if (!sight.CanSee(targetCol)) {
                remainingSightTime = requiredSightTime;
                return false;
            }

            remainingSightTime -= deltaTime;

            return remainingSightTime <= 0f;
        }
    }
}