using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    /// <summary>
    /// Represents root motion extracted from the animator for a single frame.<br/>
    /// Contains positional and rotational deltas to be applied externally.
    /// </summary>
    internal readonly struct RootMotionData {
        /// <summary>
        /// The positional movement delta produced by the animator this frame.
        /// </summary>
        public readonly Vector3 DeltaPosition;

        /// <summary>
        /// The rotational delta produced by the animator this frame.
        /// </summary>
        public readonly Quaternion DeltaRotation;

        /// <summary>
        /// Creates a new instance of root motion data using animator-provided deltas.
        /// </summary>
        public RootMotionData(Vector3 deltaPosition, Quaternion deltaRotation) {
            DeltaPosition = deltaPosition;
            DeltaRotation = deltaRotation;
        }
    }
}