using System;
using CocaCopa.StateMachine;
using UnityEngine;
using HoldMyBeer.Zombies.Contracts;

namespace HoldMyBeer.Zombies.Unity {
    public class ZombieContext : MonoBehaviour {
        [SerializeField] private Transform target;
        [SerializeField] private AIPathConfigAsset pathConfig;

        private static readonly string ScriptName = $"[{nameof(ZombieContext)}]";

        public Transform Self { get; private set; }
        public ITarget Target { get; private set; }

        public AIPath Path { get; private set; }
        public AILocomotion Locomotion { get; private set; }
        public AIAttack Attack { get; private set; }
        public AISightStimulus SightStimulus { get; private set; }
        public AIAnimator Animator { get; private set; }

        public void Create() {
            Self = transform;
            Target = target.GetComponent<ITarget>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(ITarget)}' component from '{nameof(target)}' reference. Source Obj: {name}");
            Path = new AIPath(pathConfig.Config);
            Locomotion = GetComponent<AILocomotion>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AILocomotion)}' component. Source Obj: {name}");
            Attack = GetComponent<AIAttack>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AIAttack)}' component. Source Obj: {name}");
            SightStimulus = GetComponent<AISightStimulus>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AISightStimulus)}' component. Source Obj: {name}");
            Animator = GetComponentInChildren<AIAnimator>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AIAnimator)}' component. Source Obj: {name}");
        }
    }
}