using UnityEngine;

namespace HoldMyBeer.AI {
    public abstract class AIBehaviourComposer : MonoBehaviour {
        public abstract AIStateSetup Compose(AIContext context, AIBrain brain);
    }

    public readonly struct AIStateSetup {
        public readonly IAIState MovementState;
        public readonly IAIState CombatState;

        public AIStateSetup(IAIState movementState, IAIState combatState) {
            MovementState = movementState;
            CombatState = combatState;
        }
    }
}