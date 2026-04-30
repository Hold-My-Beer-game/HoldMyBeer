using UnityEngine;

namespace HoldMyBeer.AI {
    public sealed class AIContext {
        public Transform Self { get; }
        public Transform Target { get; }

        public AIPath Path { get; }
        public AILocomotion Locomotion { get; }
        public AIAttack Attack { get; }
        public AISightStimulus SightStimulus { get; }
        public AIAnimator Animator { get; }

        public AIContext(Transform self, Transform target, AIPath path, AILocomotion locomotion, AIAttack attack, AISightStimulus sightStimulus, AIAnimator animator) {
            Self = self;
            Target = target;
            Path = path;
            Locomotion = locomotion;
            Attack = attack;
            SightStimulus = sightStimulus;
            Animator = animator;
        }
    }
}