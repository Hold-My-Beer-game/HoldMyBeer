using System;
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
        [SerializeField] private Transform hidePoint;
        [SerializeField] private float stopDistance = 5f;

        private IStateMachineBrain brain;
        private ScreamerContext context;
        private ScreamerHideState hideState;
        private DeathState deathState;

        public StateSetup Compose(IStateMachineBrain brainRef) {
            brain = brainRef;
            context = GetComponent<ScreamerContext>();
            context.Create();

            var idleMovementState = new IdleMovementState(context);
            var standUpState = new ScreamerAlertState(context, screamAnimPlayPercentage, screamOrigin.position, screamRange, screamMask);
            hideState = new ScreamerHideState(context, hidePoint.position, stopDistance);
            deathState = new DeathState(context);

            var toAlertState = new StateTransition(() => context.SightStimulus.CanSee(context.Target.Col), standUpState);
            var toHideState = new StateTransition(() => standUpState.ScreamCompleted, hideState);

            brain.AddMovementTransition(idleMovementState, toAlertState);
            brain.AddMovementTransition(standUpState, toHideState);

            Debug.Log($"Composed | Target Col Null: {context.Target.Col == null}");

            return new StateSetup(idleMovementState, new IdleCombatState());
        }

        private void Update() {
            Despawn();
        }

        private void Despawn() {
            if (hideState == null || !hideState.OnHidePos) { return; }

            enabled = true;
            gameObject.SetActive(false);
        }

        public void TakeDamage(float value) {
            context.Health.TakeDamage(value);
            if (context.Health.CurrentHealth <= 0f) {
                brain.RemoveAllTransitions();
                brain.ForceMovementState(deathState);
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(screamOrigin.position, screamRange);
        }
    }
}