using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;

public class MusicEvents : MonoBehaviour
{
    [field: Header("Soundtracks")]

    [field: SerializeField] public EventReference Tension { get; private set; }
    [field: SerializeField] public EventReference Aggression { get; private set; }
    [field: SerializeField] public EventReference DancingInThe90s { get; private set; }

    public static MusicEvents instance {  get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Music Event managers on the scene");
        }
        instance = this;
    }
}
