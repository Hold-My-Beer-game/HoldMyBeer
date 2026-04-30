using UnityEngine;

namespace CocaCopa.StateMachine {
    /// <summary>
    /// Contains path-following information for the agent's current NavMesh path progress.
    /// </summary>
    public readonly struct PathFollowData {
        /// <summary>
        /// All corner points in the calculated path.
        /// </summary>
        public readonly Vector3[] CornerPoints;

        /// <summary>
        /// Index of the current corner being followed.
        /// </summary>
        public readonly int CurrCornerIndex;

        /// <summary>
        /// Normalized direction from the current position to the current corner.
        /// </summary>
        public readonly Vector3 DirToCorner;

        /// <summary>
        /// Distance from the current position to the current corner.
        /// </summary>
        public readonly float DistToCorner;

        /// <summary>
        /// Whether the current corner is the final corner in the path.
        /// </summary>
        public bool IsAtFinalCorner => CornerPoints[CurrCornerIndex].Equals(CornerPoints[^1]);

        public PathFollowData(Vector3[] cornerPoints, int targetCornerIndex, Vector3 dirToCorner, float distToCorner) {
            CornerPoints = cornerPoints;
            CurrCornerIndex = targetCornerIndex;
            DirToCorner = dirToCorner;
            DistToCorner = distToCorner;
        }
    }
}