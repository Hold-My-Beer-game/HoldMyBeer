using UnityEngine;

namespace HoldMyBeer.AI {
    public class WalkerZombie : AIBehaviourComposer {
        [Header("Movement Layer")]
        [SerializeField] [Min(0.01f)] private float pathRefreshInterval;
        [SerializeField] [Min(0.01f)] private float stopDistance;
        [SerializeField] [Min(0f)] private float requiredSightTime;

        [Header("Combat Layer")]
        [SerializeField] private float attackTriggerRange;

        public override AIStateSetup Compose(AIContext context, AIBrain brain) {
            var targetCol = context.Target.GetComponent<Collider>();

            var idleState = new IdleMovementState(context);
            var chaseState = new ChaseTargetState(context, pathRefreshInterval, stopDistance);
            var delayedSightTransition = new DelayedSightTransition(context.SightStimulus, targetCol, requiredSightTime, chaseState);

            brain.AddMovementTransition(idleState, delayedSightTransition);
            brain.AddMovementTransition(chaseState, new StateTransition(() => !context.SightStimulus.CanSee(targetCol), idleState));

            return new AIStateSetup(idleState, null);
        }
    }
}