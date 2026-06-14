using HoldMyBeer.Audio;

namespace HoldMyBeer.UI {
    public class SettingsController {
        private readonly UINavigation nav;
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        
        public SettingsController(UINavigation nav) {
            this.nav = nav;
            masterVolume = AudioManager.instance.MasterVolume;
            musicVolume = AudioManager.instance.MusicVolume;
            sfxVolume = AudioManager.instance.SFXVolume;
        }

        public void SetMaster(float v) {
            masterVolume = AudioManager.instance.MasterVolume = v;
        }
        

        public void SetMusic(float v) {
            musicVolume = AudioManager.instance.MusicVolume = v;
        }

        public void SetSfx(float v) {
            sfxVolume = AudioManager.instance.SFXVolume = v;
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