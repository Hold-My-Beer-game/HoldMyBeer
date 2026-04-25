using UnityEngine;

namespace HoldMyBeer.AI {
    internal readonly struct RootMotionData {
        public readonly Vector3 DeltaPosition;
        public readonly Quaternion DeltaRotation;

        public RootMotionData(Vector3 deltaPosition, Quaternion deltaRotation) {
            DeltaPosition = deltaPosition;
            DeltaRotation = deltaRotation;
        }
    }
}