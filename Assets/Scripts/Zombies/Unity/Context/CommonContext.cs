using System;
using CocaCopa.StateMachine;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    [RequireComponent(typeof(AILocomotion))]
    [RequireComponent(typeof(AISightStimulus))]
    [RequireComponent(typeof(AIHealth))]
    internal abstract class CommonContext : MonoBehaviour {
        [SerializeField] private Transform target;
        [SerializeField] private AIPathConfigAsset pathConfig;

        private static readonly string ScriptName = $"[{nameof(CommonContext)}]";

        internal Transform Self { get; private set; }
        internal Collider SelfCol { get; private set; }
        internal ITarget Target { get; private set; }

        internal AIPath Path { get; private set; }
        internal AILocomotion Locomotion { get; private set; }
        internal AISightStimulus SightStimulus { get; private set; }
        internal AIAnimatorBase AnimatorBase { get; private set; }
        internal AIHealth Health { get; private set; }
        internal PatrolArea PatrolArea { get; private set; }

        protected abstract void CreateContext();

        internal void Create() {
            CreateContext();
            CreateCommonContext();
        }

        internal void SetTarget(ITarget targetRef) {
            if (!target) { Target = targetRef; }
            else { Debug.LogWarning($"{ScriptName} Tried to set a target reference but it was already set through the inspector. New reference was ignored."); }
        }

        internal void SetPatrolArea(PatrolArea area) {
            PatrolArea = area ?? throw new ArgumentNullException($"{ScriptName} {nameof(area)}");
        }

        private void CreateCommonContext() {
            if (target) { Target = target.GetComponent<ITarget>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(ITarget)}' component from '{nameof(target)}' reference. Source Obj: {name}"); }

            Self = transform;
            SelfCol = Self.GetComponent<Collider>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(Collider)}' component from 'root' object. Source Obj: {name}");
            Path = new AIPath(pathConfig.Config);
            Locomotion = GetComponent<AILocomotion>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AILocomotion)}' component. Source Obj: {name}");
            SightStimulus = GetComponent<AISightStimulus>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AISightStimulus)}' component. Source Obj: {name}");
            AnimatorBase = GetComponentInChildren<AIAnimatorBase>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AIAnimatorBase)}' component. Source Obj: {name}");
            Health = GetComponent<AIHealth>() ?? throw new NullReferenceException($"{ScriptName} Could not fetch '{nameof(AIHealth)}' component. Source Obj: {name}");
        }
    }
}