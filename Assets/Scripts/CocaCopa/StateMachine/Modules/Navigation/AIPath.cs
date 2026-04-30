using UnityEngine;
using UnityEngine.AI;

namespace CocaCopa.StateMachine {
    /// <summary>
    /// Calculates and tracks traversal progress along a NavMesh path.
    /// </summary>
    public class AIPath {
        private readonly NavMeshPath path;
        private readonly AIPathConfig pathConfig;

        private int currentCornerIndex;
        private bool hasValidPath;

        /// <summary>
        /// Creates a new AI path evaluator using the provided path configuration.
        /// </summary>
        public AIPath(AIPathConfig pathConfig) {
            path = new NavMeshPath();
            this.pathConfig = pathConfig;
        }

        /// <summary>
        /// Attempts to calculate a valid NavMesh path between the given start and end positions.<br/>
        /// Initializes traversal to the first reachable corner if successful.
        /// </summary>
        public void CalculatePath(Vector3 start, Vector3 end) {
            bool startOnMesh = NavMesh.SamplePosition(start, out NavMeshHit startHit, pathConfig.NavSampleDistance, pathConfig.AreaMask);
            bool endOnMesh = NavMesh.SamplePosition(end, out NavMeshHit endHit, pathConfig.NavSampleDistance, pathConfig.AreaMask);

            if (!startOnMesh || !endOnMesh) {
                hasValidPath = false;
                currentCornerIndex = 0;
                return;
            }

            bool pathFound = NavMesh.CalculatePath(startHit.position, endHit.position, pathConfig.AreaMask, path);

            if (!pathFound || path.status == NavMeshPathStatus.PathInvalid || path.corners.Length == 0) {
                hasValidPath = false;
                currentCornerIndex = 0;
                return;
            }

            hasValidPath = true;
            currentCornerIndex = path.corners.Length > 1 ? 1 : 0;
        }

        /// <summary>
        /// Updates path corner traversal based on the given position and returns the normalized
        /// direction from the current position to the next path corner.
        /// </summary>
        /// <param name="currentPosition">The position to calculate against.</param>
        /// <returns>Vector3.zero if the path is invalid, completed, or no movement is needed.</returns>
        public Vector3 GetDirection(Vector3 currentPosition) {
            if (!hasValidPath || path.corners.Length < 2) { return Vector3.zero; }

            while (currentCornerIndex < path.corners.Length) {
                Vector3 toCorner = path.corners[currentCornerIndex] - currentPosition;
                toCorner.y = 0f;

                if (toCorner.sqrMagnitude > pathConfig.WaypointReachDistance * pathConfig.WaypointReachDistance) { break; }

                currentCornerIndex++;
            }

            if (currentCornerIndex >= path.corners.Length) { return Vector3.zero; }

            Vector3 nextCorner = path.corners[currentCornerIndex];
            Vector3 toNextCorner = nextCorner - currentPosition;

            float distanceToCorner = toNextCorner.magnitude;

            if (distanceToCorner <= 0.0001f) { return Vector3.zero; }

            return toNextCorner / distanceToCorner;
        }

        /// <summary>
        /// Updates path corner traversal based on the given position and returns detailed path-following data.
        /// </summary>
        /// <param name="currentPosition">The current world-space position of the agent.</param>
        /// <returns>Path-following data containing corners, current corner index, movement direction, and distance to the current corner.</returns>
        public PathFollowData EvaluatePathProgress(Vector3 currentPosition) {
            if (!hasValidPath || path.corners.Length < 2) {
                return new PathFollowData(
                    path.corners, -1, Vector3.zero, 0f
                );
            }

            while (currentCornerIndex < path.corners.Length) {
                Vector3 toCorner = path.corners[currentCornerIndex] - currentPosition;
                toCorner.y = 0f;

                if (toCorner.sqrMagnitude > pathConfig.WaypointReachDistance * pathConfig.WaypointReachDistance) { break; }

                currentCornerIndex++;
            }

            if (currentCornerIndex >= path.corners.Length) {
                return new PathFollowData(
                    path.corners, currentCornerIndex, Vector3.zero, 0f
                );
            }

            Vector3 nextCorner = path.corners[currentCornerIndex];
            Vector3 toNextCorner = nextCorner - currentPosition;

            float distanceToCorner = toNextCorner.magnitude;

            if (distanceToCorner <= 0.0001f) {
                return new PathFollowData(
                    path.corners, currentCornerIndex, Vector3.zero, 0f
                );
            }

            return new PathFollowData(
                path.corners, currentCornerIndex, toNextCorner / distanceToCorner, distanceToCorner
            );
        }
    }
}