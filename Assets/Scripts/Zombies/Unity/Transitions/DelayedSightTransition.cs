using System;
using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    public sealed class DelayedSightTransition : IStateTransition {
        private static readonly string ScriptName = $"[{nameof(DelayedSightTransition)}]";

        private readonly ZombieContext context;
        private readonly float requiredSightTime;

        private float remainingSightTime;

        public bool CanTransition { get; private set; }
        public IState TargetState { get; }

        public DelayedSightTransition(ZombieContext context, float requiredSightTime, IState targetState) {
            this.context = context;
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
            if (!context.SightStimulus.CanSee(context.Target.Col)) {
                remainingSightTime = requiredSightTime;
                return false;
            }

            remainingSightTime -= deltaTime;

            return remainingSightTime <= 0f;
        }
    }
}