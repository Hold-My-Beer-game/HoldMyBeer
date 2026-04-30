using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    public sealed class AIAttack : MonoBehaviour {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float hitboxRadius;

        private static readonly string ScriptName = $"[{nameof(AIAttack)}]";

        private readonly Collider[] attackResultsBuffer = new Collider[5];

        public void TriggerRightHandAttack(Collider target) {
            int hitCount = Physics.OverlapSphereNonAlloc(attackPoint.position, hitboxRadius, attackResultsBuffer);
            if (hitCount == 0) { return; }

            for (int i = 0; i < hitCount; i++) {
                Collider result = attackResultsBuffer[i];
                if (!ReferenceEquals(result, target)) { continue; }

                // if (!result.TryGetComponent(out MeshRenderer rend)) { throw new NullReferenceException($"{ScriptName} Could not fetch 'ComponentName' from target collider"); }

                if (!result.name.Contains("Target")) { continue; }
                Vector3 dir = (result.transform.position - transform.position).normalized;
                result.transform.position += dir;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (!attackPoint) { return; }

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, hitboxRadius);
        }
#endif
    }
}