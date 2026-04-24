using UnityEngine;

namespace HoldMyBeer.AI {
    public readonly struct PathFollowData {
        public readonly Vector3[] CornerPoints;
        public readonly int CurrCornerIndex;
        public readonly Vector3 DirToCorner;
        public readonly float DistToCorner;

        public bool IsAtFinalCorner => CornerPoints[CurrCornerIndex].Equals(CornerPoints[^1]);

        public PathFollowData(Vector3[] cornerPoints, int targetCornerIndex, Vector3 dirToCorner, float distToCorner) {
            CornerPoints = cornerPoints;
            CurrCornerIndex = targetCornerIndex;
            DirToCorner = dirToCorner;
            DistToCorner = distToCorner;
        }
    }
}