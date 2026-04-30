using UnityEngine;

namespace HoldMyBeer.AI {
    public class WalkerZombie : AIBehaviourComposer {
        [Header("General")]
        [SerializeField] [Min(0.01f)] private float pathRefreshInterval;
        [SerializeField] [Min(0.01f)] private float stopDistance;

        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints = new Transform[5];

        [Header("Idle -> Chase")]
        [SerializeField] [Min(0f)] private float requiredSightTime;

        [Header("Normal Attack")]
        [SerializeField] private float normalAttackCooldown;
        [SerializeField] private float normalAttackTriggerRange;

        private Collider targetCol;
        private Vector3[] patrolVectorPoints;

        public override AIStateSetup Compose(AIContext context, AIBrain brain) {
            patrolVectorPoints = TransformsToVectors(patrolPoints);
            targetCol = context.Target.GetComponent<Collider>();

            IAIState defaultMovementState = MovementLayer(context, brain);
            // IAIState defaultCombatState = CombatLayer(context, brain);

            // return new AIStateSetup(defaultMovementState, defaultCombatState);
            return new AIStateSetup(defaultMovementState, null);
        }

        /// <summary>
        /// Composes the movement layer of the AI character
        /// </summary>
        /// <returns>The entry (default) state for the movement layer</returns>
        private IAIState MovementLayer(AIContext context, AIBrain brain) {
            var idleState = new IdleMovementState(context);
            var patrolState = new PatrolAreaState(context, patrolVectorPoints, 0f, 3f, stopDistance);
            var chaseState = new ChaseTargetState(context, pathRefreshInterval, stopDistance);
            var searchState = new SearchTargetState(context, stopDistance);

            var delayedSeenToChase = new DelayedSightTransition(context.SightStimulus, targetCol, requiredSightTime, chaseState);
            var instantSeenToChase = new StateTransition(() => context.SightStimulus.CanSee(targetCol), chaseState);

            var chaseToSearch = new StateTransition(() => !context.SightStimulus.CanSee(targetCol), searchState);
            var searchToPatrol = new StateTransition(() => searchState.OnLastKnownPos, patrolState);

            brain.AddMovementTransition(patrolState, delayedSeenToChase);
            brain.AddMovementTransition(idleState, delayedSeenToChase);
            brain.AddMovementTransition(chaseState, chaseToSearch);
            brain.AddMovementTransition(searchState, searchToPatrol, instantSeenToChase);

            return patrolState;
        }

        /// <summary>
        /// Composes the combat layer of the AI character
        /// </summary>
        /// <returns>The entry (default) state for the combat layer</returns>
        private IAIState CombatLayer(AIContext context, AIBrain brain) {
            var idleCombatState = new IdleCombatState();
            var normalAttackState = new NormalAttackState(context, targetCol, normalAttackCooldown);

            var toNormalAttack = new StateTransition(IsNormalAttackValid, normalAttackState);
            var normalAttackToIdle = new StateTransition(() => !IsNormalAttackValid(), normalAttackState);

            brain.AddCombatTransition(idleCombatState, toNormalAttack);
            brain.AddCombatTransition(normalAttackState, normalAttackToIdle);

            return idleCombatState;

            bool IsNormalAttackValid() {
                float distToPlayer = Vector3.Distance(context.Self.position, context.Target.localPosition);
                bool canSeePlayer = context.SightStimulus.CanSee(targetCol);
                return canSeePlayer && distToPlayer <= normalAttackTriggerRange;
            }
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