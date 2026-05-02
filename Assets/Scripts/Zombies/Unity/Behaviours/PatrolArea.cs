using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoldMyBeer.Zombies.Unity {
    public class PatrolArea : MonoBehaviour {
        private static readonly string ScriptName = $"[{nameof(PatrolArea)}]";

        private readonly List<Transform> occupiedPoints = new();
        private readonly List<Transform> freePatrolPoints = new();

        private void Awake() {
            InitPatrolPoints();
        }

        private void InitPatrolPoints() {
            for (int i = 0; i < transform.childCount; i++) {
                Transform point = transform.GetChild(i);
                freePatrolPoints.Add(point);
            }
        }

        public Transform GetPatrolPoint() {
            int randomIndex = Random.Range(0, freePatrolPoints.Count);
            Transform point = freePatrolPoints[randomIndex];

            if (occupiedPoints.Contains(point)) { throw new InvalidOperationException($"{ScriptName} Fetched point is already occupied. Source: {name}"); }

            occupiedPoints.Add(point);
            freePatrolPoints.Remove(point);
            return point;
        }

        public bool IsOccupied(Transform point) {
            return point != null && occupiedPoints.Contains(point);
        }

        public void ReturnPatrolPoint(Transform point) {
            occupiedPoints.Remove(point);
            freePatrolPoints.Add(point);
        }

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool drawGizmos = true;
        [SerializeField] private Color areaColor = Color.orange;
        [SerializeField] private float pointRadius = 1.2f;
        [SerializeField] private Color pointColor = Color.red;

        internal bool DrawGizmos => drawGizmos;
        internal Color AreaColor => areaColor;
        internal float PointRadius => pointRadius;
        internal Color PointColor => pointColor;
#endif
    }
}