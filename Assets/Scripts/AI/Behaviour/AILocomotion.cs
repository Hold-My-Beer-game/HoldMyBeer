using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    public class AILocomotion : MonoBehaviour {
        [SerializeField] private float rotationSpeed;

        private Vector3 targetLookDir;

        /// <summary>
        /// Rotates the transform toward the given world-space vector.<br/>
        /// </summary>
        public void SetTargetLookDir(Vector3 dir) {
            targetLookDir = dir;
        }

        public void ApplyRootMotionDelta(Vector3 delta) {
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