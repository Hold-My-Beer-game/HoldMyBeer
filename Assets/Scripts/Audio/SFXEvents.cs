using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

public class SFXEvents : MonoBehaviour
{
    [field: Header("Player SFX")]

    [field: SerializeField] public EventReference PlayerSteps { get; private set; }
    [field: SerializeField] public EventReference PlayerDrink { get; private set; }
    [field: SerializeField] public EventReference PlayerDamaged { get; private set; }
    [field: SerializeField] public EventReference PlayerDeath { get; private set; }

    [field: Header("Shotgun SFX")]

    [field: SerializeField] public EventReference BulletHit { get; private set; }
    [field: SerializeField] public EventReference DryFire { get; private set; }
    [field: SerializeField] public EventReference ShotgunFire { get; private set; }
    [field: SerializeField] public EventReference ShotgunReload { get; private set; }

    [field: Header("Zombie SFX")]

    [field: SerializeField] public EventReference ScreamerZombie { get; private set; }
    [field: SerializeField] public EventReference ScreamerRun { get; private set; }
    [field: SerializeField] public EventReference ZombieDeath { get; private set; }
    [field: SerializeField] public EventReference ZombieEat { get; private set; }
    [field: SerializeField] public EventReference ZombieMovement { get; private set; }
    [field: SerializeField] public EventReference ZombieScream { get; private set; }

    [field: Header("Misc")]
    [field: SerializeField] public EventReference LowHpHeartbeat { get; private set; }
    [field: SerializeField] public EventReference LightBreak { get; private set; }
    [field: SerializeField] public EventReference WindAmbience { get; private set; }

    public static SFXEvents instance { get; private set; }

    private void Awake() {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD SFX Event managers on the scene");
        }
        instance = this;
    }
}
