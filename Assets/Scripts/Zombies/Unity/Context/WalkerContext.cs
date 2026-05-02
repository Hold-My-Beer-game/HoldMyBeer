using System;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class WalkerContext : CommonContext {
        private static readonly string ScriptName = $"[{nameof(WalkerContext)}]";

        internal AIAttack Attack { get; private set; }
        internal WalkerAnimator Animator { get; private set; }

        protected override void CreateContext() {
            Attack = GetComponent<AIAttack>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AIAttack)}' component. Source Obj: {name}");
            Animator = GetComponentInChildren<WalkerAnimator>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(WalkerAnimator)}' component. Source Obj: {name}");
        }
    }
}