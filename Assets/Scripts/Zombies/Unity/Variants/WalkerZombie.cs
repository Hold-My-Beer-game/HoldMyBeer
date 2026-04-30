using CocaCopa.StateMachine;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    public class WalkerZombie : MonoBehaviour, IStateMachineComposer, IEnemy {
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

        private Vector3[] patrolVectorPoints;

        public StateSetup Compose(IStateMachineBrain brain) {
            ZombieContext zombieContext = CreateAndInitContext();
            patrolVectorPoints = TransformsToVectors(patrolPoints);

            IState defaultMovementState = MovementLayer(zombieContext, brain);
            IState defaultCombatState = CombatLayer(zombieContext, brain);

            return new StateSetup(defaultMovementState, defaultCombatState);
            // return new StateSetup(defaultMovementState, null);
        }

        private ZombieContext CreateAndInitContext() {
            var context = GetComponent<ZombieContext>();
            context.Create();
            return context;
        }

        /// <summary>
        /// Composes the movement layer of the AI character
        /// </summary>
        /// <returns>The entry (default) state for the movement layer</returns>
        private IState MovementLayer(ZombieContext context, IStateMachineBrain brain) {
            var idleState = new IdleMovementState(context);
            var patrolState = new PatrolAreaState(context, patrolVectorPoints, 0f, 3f, stopDistance);
            var chaseState = new ChaseTargetState(context, pathRefreshInterval, stopDistance);
            var searchState = new SearchTargetState(context, stopDistance);

            var delayedSeenToChase = new DelayedSightTransition(context, requiredSightTime, chaseState);
            var instantSeenToChase = new StateTransition(() => context.SightStimulus.CanSee(context.Target.Col), chaseState);

            var chaseToSearch = new StateTransition(() => !context.SightStimulus.CanSee(context.Target.Col), searchState);
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
        private IState CombatLayer(ZombieContext context, IStateMachineBrain brain) {
            var idleCombatState = new IdleCombatState();
            var normalAttackState = new NormalAttackState(context, normalAttackCooldown);

            var toNormalAttack = new StateTransition(IsNormalAttackValid, normalAttackState);
            var normalAttackToIdle = new StateTransition(() => !IsNormalAttackValid(), idleCombatState);

            brain.AddCombatTransition(idleCombatState, toNormalAttack);
            brain.AddCombatTransition(normalAttackState, normalAttackToIdle);

            return idleCombatState;

            bool IsNormalAttackValid() {
                float distToPlayer = Vector3.Distance(context.Self.position, context.Target.Col.transform.position);
                bool canSeePlayer = context.SightStimulus.CanSee(context.Target.Col);
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

        public void TakeDamage(float value) {
            //
        }
    }
}