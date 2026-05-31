using System;
using CocaCopa.StateMachine;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    [RequireComponent(typeof(WalkerContext))]
    public class WalkerZombie : MonoBehaviour, IStateMachineComposer, IEnemy, IScreamAffected {
        [Header("Pathing")]
        [SerializeField] [Min(0.01f)] private float pathRefreshInterval;
        [SerializeField] [Min(0.01f)] private float stopDistance;

        [Header("Patrol")]
        [SerializeField] private float timeBeforeMoving = 3f;

        [Header("Idle -> Chase")]
        [SerializeField] [Min(0f)] private float requiredSightTime;

        [Header("Normal Attack")]
        [SerializeField] private float normalAttackCooldown;
        [SerializeField] private float normalAttackTriggerRange;

        private IStateMachineBrain brain;
        private WalkerContext context;
        private SearchTargetState searchState;
        private DeathState deathState;
        private IdleCombatState idleCombatState;
        private IdleMovementState idleMovementState;
        private ChaseTargetState chaseState;
        private PatrolAreaState patrolState;

        public enum ZombieState {
            Idle, Walk, Aggro, Dead
        }

        private ZombieState currState;

        public event Action<GameObject, ZombieState> OnStateChange;
        public event Action<GameObject> OnTakeDamage;

        private void Start() {
            StateChangeManagement();
        }

        private void StateChangeManagement() {
            context.Animator.OnDeathAnimationStart += () => {
                currState = ZombieState.Dead;
                OnStateChange?.Invoke(gameObject, currState);
            };
            context.Animator.OnIdleAnimationStart += () => {
                currState = ZombieState.Idle;
                OnStateChange?.Invoke(gameObject, currState);
            };
            context.Animator.OnWalkAnimationStart += () => {
                if (currState != ZombieState.Aggro) {
                    currState = ZombieState.Walk;
                    OnStateChange?.Invoke(gameObject, currState);
                }
            };
            brain.OnStateEnterMovement += (state) => {
                if (state.Id == chaseState.Id) {
                    currState = ZombieState.Aggro;
                    OnStateChange?.Invoke(gameObject, currState);
                }
                if (state.Id == searchState.Id) {
                    currState = ZombieState.Walk;
                    OnStateChange?.Invoke(gameObject, currState);
                }
            };
        }

        public StateSetup Compose(IStateMachineBrain brainRef) {
            brain = brainRef;
            CreateAndInitContext();

            IState defaultMovementState = MovementLayer();
            IState defaultCombatState = CombatLayer();

            return new StateSetup(defaultMovementState, defaultCombatState);
        }

        private void CreateAndInitContext() {
            context = GetComponent<WalkerContext>();
            context.Create();
        }

        /// <summary>
        /// Composes the movement layer of the AI character
        /// </summary>
        /// <returns>The entry (default) state for the movement layer</returns>
        private IState MovementLayer() {
            idleMovementState = new IdleMovementState(context);
            patrolState = new PatrolAreaState(context, MoveMode.Walk, timeBeforeMoving, stopDistance);
            chaseState = new ChaseTargetState(context, MoveMode.Walk, pathRefreshInterval, stopDistance);
            searchState = new SearchTargetState(context, MoveMode.Walk, stopDistance);
            deathState = new DeathState(context);

            var delayedSeenToChase = new DelayedSightTransition(context, requiredSightTime, chaseState);
            var instantSeenToChase = new StateTransition(() => context.SightStimulus.CanSee(context.Target.Col), chaseState);

            var chaseToSearch = new StateTransition(() => !context.SightStimulus.CanSee(context.Target.Col), searchState);
            var searchToPatrol = new StateTransition(() => searchState.OnLastKnownPos, patrolState);

            brain.AddMovementTransition(patrolState, delayedSeenToChase);
            brain.AddMovementTransition(idleMovementState, delayedSeenToChase);
            brain.AddMovementTransition(chaseState, chaseToSearch);
            brain.AddMovementTransition(searchState, searchToPatrol, instantSeenToChase);

            return patrolState;
        }

        /// <summary>
        /// Composes the combat layer of the AI character
        /// </summary>
        /// <returns>The entry (default) state for the combat layer</returns>
        private IState CombatLayer() {
            idleCombatState = new IdleCombatState();
            var normalAttackState = new WalkerNormalAttackState(context, normalAttackCooldown);

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

        public void TakeDamage(float value) {
            context.Health.TakeDamage(value);
            if (context.Health.CurrentHealth.Equals(0f)) {
                brain.RemoveAllTransitions();
                context.Animator.PlayDeath();
                brain.ForceCombatState(idleCombatState);
                brain.ForceMovementState(deathState);
            }
            context.Animator.PlayHit();
            OnTakeDamage?.Invoke(gameObject);
        }

        public void React(Vector3 screamPos, Vector3 targetPos) {
            brain.ForceMovementState(searchState);
        }
    }
}