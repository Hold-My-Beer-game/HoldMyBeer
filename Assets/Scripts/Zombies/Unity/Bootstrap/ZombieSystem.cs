using System;
using System.Collections.Generic;
using HoldMyBeer.Zombies.Contracts;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    public class ZombieSystem : MonoBehaviour {
        [Header("Target Assignment")]
        [SerializeField] private Transform target;
        [SerializeField] private Transform[] zombieHolders;

        [Header("Patrol Area Assignment")]
        [SerializeField] private PatrolConfig[] patrolConfig;

        [Serializable]
        private struct PatrolConfig {
            public PatrolArea area;
            public List<CommonContext> context;
        }

        private static readonly string ScriptName = $"[{nameof(ZombieSystem)}]";

        private void Awake() {
            if (!target) { throw new ArgumentException($"{ScriptName} {nameof(target)}"); }
            if (!target.TryGetComponent(out ITarget targetRef)) { throw new NullReferenceException($"{ScriptName} Assigned {nameof(target)} reference does not implement the {nameof(ITarget)} interface"); }

            AssignTargetToZombies(targetRef);
            AssignPatrolAreas();
        }

        private void AssignTargetToZombies(ITarget targetRef) {
            for (int i = 0; i < zombieHolders.Length; i++) {
                Transform holder = zombieHolders[i];
                for (int j = 0; j < holder.childCount; j++) {
                    Transform child = holder.GetChild(j);
                    if (!child.TryGetComponent(out CommonContext context)) { throw new NullReferenceException($"{ScriptName} Holder '{zombieHolders[i].name}' contains a child (index: {j}) that does not have a '{nameof(CommonContext)}' component attached"); }
                    context.SetTarget(targetRef);
                }
            }
        }

        private void AssignPatrolAreas() {
            for (int i = 0; i < patrolConfig.Length; i++) {
                PatrolConfig config = patrolConfig[i];
                for (int j = 0; j < config.context.Count; j++) {
                    CommonContext context = config.context[j];
                    context.SetPatrolArea(config.area);
                }
            }
        }
    }
}