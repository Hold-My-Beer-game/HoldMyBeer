using CocaCopa.StateMachine;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    [RequireComponent(typeof(ScreamerContext))]
    internal class ScreamerZombie : MonoBehaviour, IStateMachineComposer, IEnemy {
        [Header("Scream Settings")]
        [SerializeField] private Transform screamOrigin;
        [SerializeField] private LayerMask screamMask;
        [SerializeField] [Range(0f, 1f)] private float screamAnimPlayPercentage;
        [SerializeField] private float screamRange;

        [Header("General")]
        [SerializeField] private Transform[] hideSpots;
        [SerializeField] private float stopDistance = 5f;

        private ScreamerContext context;

        public StateSetup Compose(IStateMachineBrain brain) {
            context = GetComponent<ScreamerContext>();
            context.Create();

            var idleMovementState = new IdleMovementState(context);
            var standUpState = new ScreamerAlertState(context, screamAnimPlayPercentage, screamOrigin.position, screamRange, screamMask);
            var hideState = new SearchTargetState(context, MoveMode.Run, stopDistance);

            var toAlertState = new StateTransition(() => context.SightStimulus.CanSee(context.Target.Col), standUpState);
            var toHideState = new StateTransition(() => standUpState.ScreamCompleted, hideState);

            brain.AddMovementTransition(idleMovementState, toAlertState);
            brain.AddMovementTransition(standUpState, toHideState);

            return new StateSetup(idleMovementState, null);
        }

        public void TakeDamage(float value) {
            context.Health.TakeDamage(value);
        }
    }
}