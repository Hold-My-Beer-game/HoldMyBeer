using System;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

namespace Player {
    public class PlayerController : MonoBehaviour, ITarget {
        private static readonly string ScriptName = $"[{nameof(PlayerController)}]";
        
        private PlayerHealth health;
        private Collider col;

        public Collider Col => col;

        private void Awake() {
            health = GetComponent<PlayerHealth>() ?? throw new NullReferenceException($"{ScriptName} {nameof(health)}");
            col = GetComponent<Collider>() ?? throw new NullReferenceException($"{ScriptName} {nameof(col)}");
        }

        public void TakeDamage(float value) {
            health.TakeDamage(value);
        }
    }
}