using System;
using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class DelayedSightTransition : IStateTransition {
        internal DelayedSightTransition(CommonContext context, float requiredSightTime, IState targetState) {
            this.context = context;
            this.requiredSightTime = Mathf.Max(0f, requiredSightTime);

            TargetState = targetState ?? throw new ArgumentNullException($"{ScriptName} {nameof(targetState)}");

            remainingSightTime = this.requiredSightTime;
            CanTransition = false;
        }

        private static readonly string ScriptName = $"[{nameof(DelayedSightTransition)}]";

        private readonly CommonContext context;
        private readonly float requiredSightTime;

        private float remainingSightTime;

        public bool CanTransition { get; private set; }
        public IState TargetState { get; }

        public void OnSourceStateEnter() { }

        public void Tick(float deltaTime) {
            CanTransition = AllowTransition(deltaTime);
        }

        public void OnSourceStateExit() {
            CanTransition = false;
            remainingSightTime = requiredSightTime;
        }

        private bool AllowTransition(float deltaTime) {
            if (!context.SightStimulus.CanSee(context.Target.Col)) {
                remainingSightTime = requiredSightTime;
                return false;
            }

            remainingSightTime -= deltaTime;

            return remainingSightTime <= 0f;
        }
    }
}