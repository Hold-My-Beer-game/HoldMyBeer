using CocaCopa.StateMachine;
using UnityEngine;

namespace HoldMyBeer.Zombies.Unity {
    public class NormalAttackState : IState {
        public NormalAttackState(ZombieContext context, float cooldown) {
            this.context = context;
            this.cooldown = cooldown;
        }

        private readonly ZombieContext context;
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
            context.Attack.TriggerRightHandAttack(context.Target.Col);
        }

        private bool CanAttack(float deltaTime) {
            cooldownTimer -= deltaTime;

            if (cooldownTimer > 0f) { return false; }

            cooldownTimer = cooldown;
            return true;
        }
    }
}