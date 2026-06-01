using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class ScreamerAlertState : IState {
        internal ScreamerAlertState(ScreamerContext context, float maxPlayPercentage, Vector3 screamOrigin, float screamRange, LayerMask screamAffectedLayer) {
            this.context = context;
            this.maxPlayPercentage = maxPlayPercentage;
            this.screamOrigin = screamOrigin;
            this.screamRange = screamRange;
            this.screamAffectedLayer = screamAffectedLayer;
        }

        private readonly ScreamerContext context;
        private readonly float maxPlayPercentage;
        private readonly Vector3 screamOrigin;
        private readonly LayerMask screamAffectedLayer;
        private readonly float screamRange;

        private readonly Collider[] nearbyZombiesBuffer = new Collider[20];

        internal bool ScreamCompleted { get; private set; }

        public string Id => nameof(ScreamerAlertState);

        public void Enter() {
            ScreamCompleted = false;
            context.Animator.OnRootMotionDataUpdated += Animator_OnRootMotionDataUpdated;
            context.Animator.OnZombieScreamStart += Animator_OnZombieScream;
            context.Animator.PlayStandUp();
        }

        public void Tick(float deltaTime) {
            float screamPercentage = context.Animator.GetScreamStatePercentage();
            if (screamPercentage >= maxPlayPercentage) { ScreamCompleted = true; }
        }

        public void Exit() {
            context.Animator.OnRootMotionDataUpdated -= Animator_OnRootMotionDataUpdated;
        }

        private void Animator_OnRootMotionDataUpdated(RootMotionData data) {
            context.Locomotion.ApplyRootMotionDelta(data.DeltaPosition);
        }

        private void Animator_OnZombieScream() {
            int hitCount = Physics.OverlapSphereNonAlloc(screamOrigin, screamRange, nearbyZombiesBuffer, screamAffectedLayer);

            for (int i = 0; i < hitCount; i++) {
                Collider zombieCol = nearbyZombiesBuffer[i];

                if (!zombieCol.TryGetComponent(out IScreamAffected screamAffected)) { continue; }

                Vector3 screamPos = context.Self.position;
                Vector3 targetPos = context.Target.Col.transform.position;
                screamAffected.React(screamPos, targetPos);
            }
        }
    }
}