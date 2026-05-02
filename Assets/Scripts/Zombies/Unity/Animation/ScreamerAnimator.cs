using System;
using CocaCopa.Unity;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class ScreamerAnimator : AIAnimatorBase {
        [Header("States")]
        [SerializeField] [AnimatorState] private string screamState;
        [SerializeField] [AnimatorState] private string runState;
        [SerializeField] [AnimatorState] private string takeDamageState;
        [SerializeField] [AnimatorState] private string deathState;

        [Header("Parameters")]
        [SerializeField] [AnimatorParameter] private string standUpTrigger;

        internal Action OnZombieScream;

        internal void PlayStandUp() {
            animator.SetTrigger(standUpTrigger);
        }

        internal float GetScreamStatePercentage() {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(screamState) ? Mathf.Clamp01(stateInfo.normalizedTime) : -1f;
        }

        /// <summary>
        /// Meant to be called through an animation event via Unity's animation system
        /// </summary>
        private void ScreamAnimationOnPosition() {
            OnZombieScream?.Invoke();
        }

        internal override void PlayIdle() { }

        internal override void PlayChase() { }

        internal override void PlayWalk() { }

        internal override void PlayRun() {
            animator.CrossFade(runState, 0.25f, 0);
        }

        internal override void PlayHit() {
            animator.Play(takeDamageState, 1, 0f);
        }

        internal override void PlayDeath() {
            animator.Play(deathState, 0, 0f);
        }
    }
}