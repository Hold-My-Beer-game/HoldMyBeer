namespace HoldMyBeer.UI {
    /// <summary>
    /// Converts gameplay systems into HUD state mutations.
    /// </summary>
    internal class HUDController {
        private readonly HUDState state;

        private readonly IPlayerStateRead playerStateRead;

        private readonly GameFlowController gameFlow;

        public void Init() {

            if (playerStateRead == null) { return; }
            playerStateRead.OnHealthChange += SetHealth;
            playerStateRead.OnAlcoholChange += SetDrunkness;
            playerStateRead.OnGoalChange += SetGoal;
            playerStateRead.OnInteract += SetInteract;
            playerStateRead.OnAmmoChange += SetAmmo;
            playerStateRead.OnLoadChange += SetLoadedAmmo;
            playerStateRead.OnDeath += SetDead;
        }

        

        public HUDController(HUDState state, IPlayerStateRead playerStateReadRef, GameFlowController gameFlowRef) {
            this.state = state;
            playerStateRead = playerStateReadRef;
            gameFlow = gameFlowRef;
        }

        // Gameplay API
        private void SetDead() {
            gameFlow.Endgame();
        }
        
        private void SetHealth(float value) {
            state.Health = value / 100f;
            state.Notify();
        }

        private void SetDrunkness(float value) {
            state.Drunkness = value;
            state.Notify();
        }

        private void SetGoal(string text) {
            state.GoalText = text;
            state.Notify();
        }

        private void SetInteract(bool visible, string text) {
            state.InteractVisible = visible;
            state.InteractText = text;
            state.Notify();
        }

        private void SetAmmo(int current) {
            state.Ammo = current;
            state.Notify();
        }

        private void SetLoadedAmmo(int current) {
            state.LoadedAmmo = current;
            state.Notify();
        }
    }
}