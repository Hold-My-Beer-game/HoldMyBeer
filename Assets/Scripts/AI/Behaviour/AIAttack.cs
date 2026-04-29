using UnityEngine;

namespace HoldMyBeer.AI {
    public sealed class AIAttack : MonoBehaviour {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float hitboxRadius;

        private readonly Collider[] attackResultsBuffer = new Collider[5];

        public void TriggerRightHandAttack() {
            int hitCount = Physics.OverlapSphereNonAlloc(attackPoint.position, hitboxRadius, attackResultsBuffer);
            if (hitCount == 0) { return; }

            for (int i = 0; i < hitCount; i++) {
                Collider result = attackResultsBuffer[i];
                // player.TakeDamage();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPoint.position, hitboxRadius);
        }
#endif
    }
}