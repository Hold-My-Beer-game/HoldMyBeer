using System;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;
using HoldMyBeer.UI;

namespace Player {
    public class PlayerController : MonoBehaviour, ITarget, IPlayerStateRead {
        private static readonly string ScriptName = $"[{nameof(PlayerController)}]";

        private PlayerHealth health;
        private PlayerInventory inventory;
        private PlayerInteract interact;
        private Collider col;
        private PlayerAttack attack;

        public Collider Col => col;

        public event Action<float> OnHealthChange;
        public event Action<float> OnAlcoholChange;
        public event Action<string> OnGoalChange;
        public event Action<bool, string> OnInteract;
        public event Action<int> OnAmmoChange;
        public event Action<int> OnLoadChange;
        
        public event Action OnDeath;

        private void Awake() {
            health = GetComponent<PlayerHealth>() ?? throw new NullReferenceException($"{ScriptName} {nameof(health)}");
            inventory = GetComponent<PlayerInventory>() ?? throw new NullReferenceException($"{ScriptName} {nameof(inventory)}");
            interact = GetComponent<PlayerInteract>() ?? throw new NullReferenceException($"{ScriptName} {nameof(interact)}");
            col = GetComponent<Collider>() ?? throw new NullReferenceException($"{ScriptName} {nameof(col)}");
            attack = GetComponent<PlayerAttack>() ?? throw new NullReferenceException($"{ScriptName} {nameof(attack)}");
            
            health.OnHealthChange += (amount) => OnHealthChange?.Invoke(amount);
            health.OnDeath += ( ) => OnDeath?.Invoke();
            inventory.OnAlcoholChange += (amount) => OnAlcoholChange?.Invoke(amount);
            interact.OnPlayerInteract += (canInteract, msg) => OnInteract?.Invoke(canInteract, msg);
            inventory.OnAmmoChange += (amount) => OnAmmoChange?.Invoke((int)amount);
            attack.OnLoadChange += (amount) => OnLoadChange?.Invoke((int)amount); 
            
        }

        public void TakeDamage(float value) {
            health.TakeDamage(value);
        }
    }
}