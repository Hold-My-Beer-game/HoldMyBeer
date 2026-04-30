using UnityEngine;

namespace HoldMyBeer.Zombies.Contracts {
    public interface ITarget {
        Collider Col { get; }
        void TakeDamage(float value);
    }
}