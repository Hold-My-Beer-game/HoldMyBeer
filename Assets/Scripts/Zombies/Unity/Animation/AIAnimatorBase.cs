using System;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    [RequireComponent(typeof(Animator))]
    internal abstract class AIAnimatorBase : MonoBehaviour {
        protected Animator animator;

        internal event Action<RootMotionData> OnRootMotionDataUpdated;

        protected virtual void Awake() {
            animator = GetComponent<Animator>();
        }

        protected virtual void OnAnimatorMove() {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            OnRootMotionDataUpdated?.Invoke(new RootMotionData(animator.deltaPosition, animator.deltaRotation));
        }

        protected static float SnapNearInteger(float value) {
            float nearestInteger = Mathf.Round(value);
            return Mathf.Abs(value - nearestInteger) <= 0.01f ? nearestInteger : value;
        }

        internal abstract void PlayIdle();
        internal abstract void PlayChase();
        internal abstract void PlayWalk();
        internal abstract void PlayRun();
        internal abstract void PlayHit();
        internal abstract void PlayDeath();
    }
}