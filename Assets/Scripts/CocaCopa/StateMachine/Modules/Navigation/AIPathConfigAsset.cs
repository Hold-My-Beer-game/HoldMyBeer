using System;
using UnityEngine;

namespace CocaCopa.StateMachine {
    [CreateAssetMenu(fileName = "AIPathConfig", menuName = "HoldMyBeer/AI/PathConfig")]
    public sealed class AIPathConfigAsset : ScriptableObject {
        [SerializeField] private AIPathConfig config;

        public AIPathConfig Config => config;
    }

    [Serializable]
    public struct AIPathConfig {
        [Tooltip("Distance threshold to consider a path corner reached and advance to the next.")]
        [SerializeField] private float waypointReachDistance;
        [Tooltip("Maximum distance used to snap positions onto the NavMesh when calculating paths.")]
        [SerializeField] private float navSampleDistance;
        [Tooltip("NavMesh areas that are considered valid for pathfinding.")]
        [SerializeField] private int areaMask;

        public float WaypointReachDistance => waypointReachDistance;
        public float NavSampleDistance => navSampleDistance;
        public int AreaMask => areaMask;
    }
}