using UnityEngine;

namespace HoldMyBeer.AI {
    public class WalkerZombie : AIBehaviourComposer {
        [Header("Movement")]
        [SerializeField] [Min(0.01f)] private float pathRefreshInterval;
        [SerializeField] [Min(0.01f)] private float stopDistance;
        [SerializeField] private Transform[] patrolPoints = new Transform[5];

        [Header("Idle -> Chase")]
        [SerializeField] [Min(0f)] private float requiredSightTime;

        [Header("Combat Layer")]
        [SerializeField] private float attackTriggerRange;

        public override AIStateSetup Compose(AIContext context, AIBrain brain) {
            Vector3[] patrolVectorPoints = TransformsToVectors(patrolPoints);
            var targetCol = context.Target.GetComponent<Collider>();

            var idleState = new IdleMovementState(context);
            var patrolState = new PatrolAreaState(context, patrolVectorPoints, 0f, 3f, stopDistance);
            var chaseState = new ChaseTargetState(context, pathRefreshInterval, stopDistance);
            var searchState = new SearchTargetState(context, stopDistance);

            var idleToChase = new DelayedSightTransition(context.SightStimulus, targetCol, requiredSightTime, chaseState);
            var chaseToSearch = new StateTransition(() => !context.SightStimulus.CanSee(targetCol), searchState);
            var searchToPatrol = new StateTransition(() => searchState.OnLastKnownPos, patrolState);
            var searchToChase = new StateTransition(() => context.SightStimulus.CanSee(targetCol), chaseState);
            var patrolToChase = new DelayedSightTransition(context.SightStimulus, targetCol, requiredSightTime, chaseState);

            brain.AddMovementTransition(patrolState, patrolToChase);
            brain.AddMovementTransition(idleState, idleToChase);
            brain.AddMovementTransition(chaseState, chaseToSearch);
            brain.AddMovementTransition(searchState, searchToPatrol, searchToChase);

            return new AIStateSetup(patrolState, null);
        }

        private static Vector3[] TransformsToVectors(Transform[] transforms) {
            var vectors = new Vector3[transforms.Length];
            for (int i = 0; i < vectors.Length; i++) {
                Vector3 pos = transforms[i].position;
                vectors[i] = pos;
            }
            return vectors;
        }
    }
}