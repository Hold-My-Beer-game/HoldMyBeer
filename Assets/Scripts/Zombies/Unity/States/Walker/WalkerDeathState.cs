using System;
using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class WalkerDeathState : IState {
        internal WalkerDeathState(WalkerContext context) {
            this.context = context;
        }

        private static readonly string ScriptName = $"[{nameof(WalkerDeathState)}]";

        private readonly WalkerContext context;

        public string Id => nameof(WalkerDeathState);

        public void Enter() {
            context.Animator.PlayDeath();
            context.SelfCol.enabled = false;
        }

        public void Tick(float deltaTime) { }
        public void Exit() { }
    }
}