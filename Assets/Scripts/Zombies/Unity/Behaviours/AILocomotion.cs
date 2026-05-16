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
            targetLookDir.y = 0;
        }

        /// <summary>
        /// Applies positional root motion delta to the transform.
        /// </summary>
        /// <param name="delta">World-space movement delta.</param>
        internal void ApplyRootMotionDelta(Vector3 delta) {
            transform.position += delta;
            Vector3 pos = transform.position;
            pos.y = GroundHeight();
            transform.position = pos;
        }

        private float GroundHeight() {
            Vector3 origin = transform.position + Vector3.up * 0.25f;
            Vector3 dir = Vector3.down;
            const float dist = 10f;
            if (Physics.Raycast(origin, dir, out RaycastHit hit, dist)) { return hit.point.y; }
            return -1f;
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