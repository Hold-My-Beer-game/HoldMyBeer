using HoldMyBeer.Audio;

namespace HoldMyBeer.UI {
    public class SettingsController {
        private readonly UINavigation nav;
        
        public SettingsController(UINavigation nav) {
            this.nav = nav;
        }

        public void SetMaster(float v) {
            AudioManager.instance.MasterVolume = v;
        }

        public void SetMusic(float v) {
            AudioManager.instance.MusicVolume = v;
        }

        public void SetSfx(float v) {
            AudioManager.instance.SFXVolume = v;
        }

        // public void SetQuality(int i) {
        //     state.QualityIndex = i;
        // }
        //
        // public void SetSensitivity(float v) {
        //     state.Sensitivity = v;
        // }

        public void Back() {
            nav.Back();
        }
    }
}