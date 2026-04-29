using UnityEngine;

namespace HoldMyBeer.AI {
    public class WalkerZombie : AIBehaviourComposer {
        [Header("Movement")]
        [SerializeField] [Min(0.01f)] private float pathRefreshInterval;
        [SerializeField] [Min(0.01f)] private float stopDistance;

        [Header("Idle -> Chase")]
        [SerializeField] [Min(0f)] private float requiredSightTime;

        [Header("Combat Layer")]
        [SerializeField] private float attackTriggerRange;

        public override AIStateSetup Compose(AIContext context, AIBrain brain) {
            var targetCol = context.Target.GetComponent<Collider>();

            var idleState = new IdleMovementState(context);
            var chaseState = new ChaseTargetState(context, pathRefreshInterval, stopDistance);
            var searchState = new SearchTargetState(context, stopDistance);

            var idleToChase = new DelayedSightTransition(context.SightStimulus, targetCol, requiredSightTime, chaseState);
            var chaseToSearch = new StateTransition(() => !context.SightStimulus.CanSee(targetCol), searchState);
            var searchToIdle = new StateTransition(() => searchState.OnLastKnownPos, idleState);
            var searchToChase = new StateTransition(() => context.SightStimulus.CanSee(targetCol), chaseState);

            brain.AddMovementTransition(idleState, idleToChase);
            brain.AddMovementTransition(chaseState, chaseToSearch);
            brain.AddMovementTransition(searchState, searchToIdle, searchToChase);

            return new AIStateSetup(idleState, null);
        }
    }
}