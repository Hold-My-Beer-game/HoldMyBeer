using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    /// <summary>
    /// Handles character rotation and movement application using externally provided root motion.<br/>
    /// Responsible only for transforming the GameObject, not deciding movement logic.
    /// </summary>
    internal sealed class AILocomotion : MonoBehaviour {
        [Tooltip("Speed at which the character rotates toward the target look direction.")]
        [SerializeField] private float rotationSpeed;

        private Vector3 targetLookDir;

        /// <summary>
        /// Sets the desired world-space direction the character should rotate toward.
        /// </summary>
        /// <param name="dir">Target direction in world space.</param>
        internal void SetTargetLookDir(Vector3 dir) {
            targetLookDir = dir;
        }

        /// <summary>
        /// Applies positional root motion delta to the transform.
        /// </summary>
        /// <param name="delta">World-space movement delta.</param>
        internal void ApplyRootMotionDelta(Vector3 delta) {
            transform.position += delta;
        }

        private void Update() {
            RotateTowards();
        }

        private void RotateTowards() {
            if (targetLookDir.sqrMagnitude < 0.0001f) { return; }

            Quaternion targetRotation = Quaternion.LookRotation(targetLookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}