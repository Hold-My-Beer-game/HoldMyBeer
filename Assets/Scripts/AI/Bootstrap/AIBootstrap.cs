using System;
using UnityEngine;

namespace HoldMyBeer.AI {
    [RequireComponent(typeof(AIBrain))]
    [RequireComponent(typeof(AIBehaviourComposer))]
    [RequireComponent(typeof(AILocomotion))]
    public class AIBootstrap : MonoBehaviour {
        [SerializeField] private AIPathConfigAsset pathConfig;
        [SerializeField] private Transform target;

        private static readonly string ScriptName = $"[{nameof(AIBootstrap)}]";

        private void Awake() {
            var brain = GetComponent<AIBrain>();
            var composer = GetComponent<AIBehaviourComposer>();

            AISightStimulus sightStimulus = GetComponent<AISightStimulus>() ?? GetComponentInChildren<AISightStimulus>();
            var attack = GetComponent<AIAttack>();
            var path = new AIPath(pathConfig.Config);
            var locomotion = GetComponent<AILocomotion>();
            var animator = GetComponentInChildren<AIAnimator>();

            if (!pathConfig) { throw new NullReferenceException($"{ScriptName} {nameof(pathConfig)}"); }
            if (!target) { throw new NullReferenceException($"{ScriptName} {nameof(target)}"); }
            if (!sightStimulus) { throw new NullReferenceException($"{ScriptName} {nameof(sightStimulus)}"); }
            if (!attack) { throw new NullReferenceException($"{ScriptName} {nameof(attack)}"); }
            if (!animator) { throw new NullReferenceException($"{ScriptName} {nameof(animator)}"); }

            var context = new AIContext(transform, target, path, locomotion, attack, sightStimulus, animator);

            AIStateSetup setup = composer.Compose(context, brain);
            brain.Init(setup.MovementState, setup.CombatState);
        }
    }
}