using System;
using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class DeathState : IState {
        internal DeathState(CommonContext context) {
            this.context = context;
        }

        private static readonly string ScriptName = $"[{nameof(DeathState)}]";

        private readonly CommonContext context;

        public string Id => nameof(DeathState);

        public void Enter() {
            context.AnimatorBase.PlayDeath();
            context.SelfCol.enabled = false;
        }

        public void Tick(float deltaTime) { }
        public void Exit() { }
    }
}