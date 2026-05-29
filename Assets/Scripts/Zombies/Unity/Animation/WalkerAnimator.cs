using System;
using CocaCopa.Unity;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class WalkerAnimator : AIAnimatorBase {
        [SerializeField] [AnimatorState] private string normalAttackState;
        [SerializeField] [AnimatorState] private string takeDamageState;
        [SerializeField] [AnimatorState] private string deathState;
        [Tooltip("Animator float parameter used to drive the locomotion blend tree value.")]
        [SerializeField] [AnimatorParameter] private string locomotionSpeedParam;
        [Tooltip("Time used to smooth changes to the locomotion speed animator parameter.")]
        [SerializeField] [Min(0f)] private float locomotionSmoothTime;

        private float currentAnimSpeed;
        private float animSpeedVelocity;

        private float targetLocomotionSpeed;

        internal event Action OnWalkAnimationStart;
        internal event Action OnIdleAnimationStart;
        internal event Action OnDeathAnimationStart;

        internal event Action NormalAttackOnDamagePos;

        private void Update() {
            UpdateLocomotionSpeedParameter();
        }

        internal override void PlayIdle() {
            if (targetLocomotionSpeed == 0f) { return; }
            OnIdleAnimationStart?.Invoke();
            targetLocomotionSpeed = 0f;
        }

        internal override void PlayWalk() {
            if (Mathf.Approximately(targetLocomotionSpeed, 1f)) { return; }
            OnWalkAnimationStart?.Invoke();
            targetLocomotionSpeed = 1f;
        }

        internal override void PlayChase() {
            targetLocomotionSpeed = 1f;
        }

        internal override void PlayRun() { }

        internal override void PlayHit() {
            animator.Play(takeDamageState, 1, 0f);
        }

        internal override void PlayDeath() {
            animator.Play(deathState, 0, 0f);
            enabled = false;
            currentAnimSpeed = 0f;
            OnDeathAnimationStart?.Invoke();
        }

        /// <summary>
        /// Meant to be called through an animation event via Unity's animation system
        /// </summary>
        private void NormalAttackOnDamagePosition() {
            NormalAttackOnDamagePos?.Invoke();
        }

        private void UpdateLocomotionSpeedParameter() {
            currentAnimSpeed = Mathf.SmoothDamp(currentAnimSpeed, targetLocomotionSpeed, ref animSpeedVelocity, locomotionSmoothTime);
            currentAnimSpeed = SnapNearInteger(currentAnimSpeed);
            animator.SetFloat(locomotionSpeedParam, currentAnimSpeed);
        }

        /// <summary>
        /// Plays the Normal Attack animation from layer 1
        /// </summary>
        internal void PlayNormalAttack() {
            animator.CrossFade(normalAttackState, 0.25f, 1);
        }
    }
}