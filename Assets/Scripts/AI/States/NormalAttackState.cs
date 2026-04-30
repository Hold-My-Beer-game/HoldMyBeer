using UnityEngine;

namespace HoldMyBeer.AI {
    public class NormalAttackState : IAIState {
        public NormalAttackState(AIContext context, Collider targetCol, float cooldown) {
            this.context = context;
            this.targetCol = targetCol;
            this.cooldown = cooldown;
        }

        private readonly AIContext context;
        private readonly Collider targetCol;
        private readonly float cooldown;

        private float cooldownTimer;

        public string Id => nameof(NormalAttackState);

        public void Enter() {
            cooldownTimer = 0f;
            context.Animator.NormalAttackOnDamagePos += Animator_NormalAttackOnDamagePos;
        }

        public void Tick(float deltaTime) {
            if (CanAttack(deltaTime)) { context.Animator.PlayNormalAttack(); }
        }

        public void Exit() {
            cooldownTimer = 0f;
            context.Animator.NormalAttackOnDamagePos -= Animator_NormalAttackOnDamagePos;
        }

        private void Animator_NormalAttackOnDamagePos() {
            context.Attack.TriggerRightHandAttack(targetCol);
        }

        private bool CanAttack(float deltaTime) {
            cooldownTimer -= deltaTime;

            if (cooldownTimer > 0f) { return false; }

            cooldownTimer = cooldown;
            return true;
        }
    }
}