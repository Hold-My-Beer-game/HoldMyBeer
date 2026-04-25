using System;
using CocaCopa.Unity;
using UnityEngine;

namespace HoldMyBeer.AI {
    [RequireComponent(typeof(Animator))]
    internal sealed class AIAnimator : MonoBehaviour {
        [SerializeField] [AnimatorParameter] private string locomotionSpeedParam;
        [SerializeField] private float locomotionSmoothTime;

        private Animator animator;
        private float currentAnimSpeed;
        private float animSpeedVelocity;

        private float targetLocomotionSpeed;

        internal event Action<RootMotionData> OnRootMotionDataUpdated;

        private void Awake() {
            animator = GetComponent<Animator>();
        }

        internal void SetTargetLocomotionSpeed(float value) {
            targetLocomotionSpeed = value;
        }

        private void OnAnimatorMove() {
            if (!animator) { throw new NullReferenceException($"[{nameof(AIAnimator)}] {nameof(animator)}"); }

            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            OnRootMotionDataUpdated?.Invoke(new RootMotionData(animator.deltaPosition, animator.deltaRotation));
        }

        private void Update() {
            UpdateLocomotionSpeedParameter();
        }

        private void UpdateLocomotionSpeedParameter() {
            currentAnimSpeed = Mathf.SmoothDamp(currentAnimSpeed, targetLocomotionSpeed, ref animSpeedVelocity, locomotionSmoothTime);
            currentAnimSpeed = SnapNearInteger(currentAnimSpeed);
            animator.SetFloat(locomotionSpeedParam, currentAnimSpeed);
        }

        private static float SnapNearInteger(float value) {
            float nearestInteger = Mathf.Round(value);
            return Mathf.Abs(value - nearestInteger) <= 0.01f ? nearestInteger : value;
        }
    }
}