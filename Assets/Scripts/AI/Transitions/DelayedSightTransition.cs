using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    public sealed class DelayedSightTransition : IStateTransition {
        private static readonly string ScriptName = $"[{nameof(DelayedSightTransition)}]";

        private readonly AISightStimulus sight;
        private readonly Collider targetCol;
        private readonly float requiredSightTime;

        private float remainingSightTime;

        public IAIState TargetState { get; }

        public DelayedSightTransition(AISightStimulus sight, Collider targetCol, float requiredSightTime, IAIState targetState) {
            this.sight = sight ?? throw new ArgumentNullException(nameof(sight));
            this.targetCol = targetCol ?? throw new ArgumentNullException(nameof(targetCol));
            this.requiredSightTime = Mathf.Max(0f, requiredSightTime);

            TargetState = targetState ?? throw new ArgumentNullException(nameof(targetState));

            remainingSightTime = this.requiredSightTime;
        }

        public bool CanTransition() {
            if (!sight.CanSee(targetCol)) {
                remainingSightTime = requiredSightTime;
                return false;
            }

            remainingSightTime -= Time.deltaTime;

            if (remainingSightTime > 0f) { return false; }

            remainingSightTime = requiredSightTime;
            return true;
        }
    }
}