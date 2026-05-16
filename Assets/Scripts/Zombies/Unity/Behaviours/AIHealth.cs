using System;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    internal sealed class AIHealth : MonoBehaviour {
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;

        public float CurrentHealth => currentHealth;

        private void Awake() {
            currentHealth = maxHealth;
        }

        internal void TakeDamage(float value) {
            float health = Mathf.Max(0f, currentHealth - value);
            currentHealth = health;
        }
    }
}