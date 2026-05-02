namespace HoldMyBeer.UI {
    public class SettingsController {
        private readonly SettingsState state;
        private readonly UINavigation nav;

        public SettingsController(SettingsState state, UINavigation nav) {
            this.state = state;
            this.nav = nav;
        }

        public void SetMaster(float v) {
            state.MasterVolume = v;
            state.Notify();
        }

        public void SetMusic(float v) {
            state.MusicVolume = v;
            state.Notify();
        }

        public void SetSfx(float v) {
            state.SfxVolume = v;
            state.Notify();
        }

        public void SetQuality(int i) {
            state.QualityIndex = i;
            state.Notify();
        }
        
        public void SetSensitivity(float v) {
            state.Sensitivity = v;
            state.Notify();
        }

        public void Back() {
            nav.Back();
        }
    }
}