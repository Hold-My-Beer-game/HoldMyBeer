using System;

namespace HoldMyBeer.UI {
    public class SettingsState {
        public float MasterVolume;
        public float MusicVolume;
        public float SfxVolume;

        public int QualityIndex;
        public float Sensitivity;

        public event Action OnChanged;

        public void Notify() => OnChanged?.Invoke();
    }
}