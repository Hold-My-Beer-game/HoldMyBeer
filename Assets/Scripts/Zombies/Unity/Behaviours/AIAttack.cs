using System;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    public sealed class AIAttack : MonoBehaviour {
        [SerializeField] private float damageAmount = 20f;
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

                if (!result.TryGetComponent(out ITarget player)) { throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(ITarget)}' from target collider"); }

                player.TakeDamage(damageAmount);
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