using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

public class TestPlayer : MonoBehaviour, ITarget {
    public Collider Col { get; private set; }

    private void Awake() {
        Col = GetComponent<Collider>();
    }

    public void TakeDamage(float value) {
        //
    }
}