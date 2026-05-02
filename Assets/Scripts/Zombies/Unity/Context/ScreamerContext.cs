using System;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class ScreamerContext : CommonContext {
        private static readonly string ScriptName = $"[{nameof(ScreamerContext)}]";

        internal ScreamerAnimator Animator { get; private set; }

        protected override void CreateContext() {
            Animator = GetComponentInChildren<ScreamerAnimator>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(ScreamerAnimator)}' component. Source Obj: {name}");
        }
    }
}