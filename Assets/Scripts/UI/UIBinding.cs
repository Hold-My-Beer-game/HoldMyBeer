using UnityEngine.UIElements;

namespace HoldMyBeer.UI {
    public static class UIBinding {
        public static void BindMainMenu(VisualElement root, MainMenuController c) {
            VisualElement view = root.Q("MainMenu");
            if (view == null) return;

            view.Q<Button>("btn-NewGame")?.RegisterCallback<ClickEvent>(_ => c.StartGame());
            view.Q<Button>("btn-Settings")?.RegisterCallback<ClickEvent>(_ => c.OpenSettings());
            view.Q<Button>("btn-About")?.RegisterCallback<ClickEvent>(_ => c.OpenAbout());
            view.Q<Button>("btn-Quit")?.RegisterCallback<ClickEvent>(_ => c.Quit());
        }

        public static void BindPause(VisualElement root, GameFlowController c) {
            VisualElement view = root.Q("Pause");
            if (view == null) return;

            view.Q<Button>("RestartButton")?.RegisterCallback<ClickEvent>(_ => c.Restart());
            view.Q<Button>("MainMenuButton")?.RegisterCallback<ClickEvent>(_ => c.MainMenu());
            view.Q<Button>("SettingsButton")?.RegisterCallback<ClickEvent>(_ => c.Settings());
            view.Q<Button>("QuitButton")?.RegisterCallback<ClickEvent>(_ => c.Quit());
            // todo pause exit button
        }

        public static void BindEndgame(VisualElement root, GameFlowController c) {
            VisualElement view = root.Q("Endgame");
            if (view == null) return;

            view.Q<Button>("RestartButton")?.RegisterCallback<ClickEvent>(_ => c.Restart());
            view.Q<Button>("MainMenuButton")?.RegisterCallback<ClickEvent>(_ => c.MainMenu());
            view.Q<Button>("SettingsButton")?.RegisterCallback<ClickEvent>(_ => c.Settings());
            view.Q<Button>("QuitButton")?.RegisterCallback<ClickEvent>(_ => c.Quit());
        }

        public static void BindSettings(VisualElement root, SettingsController c) {
            VisualElement view = root.Q("Settings");
            if (view == null) return;

            view.Q<Slider>("master-slider")?.RegisterValueChangedCallback(e => c.SetMaster(e.newValue));
            view.Q<Slider>("music-slider")?.RegisterValueChangedCallback(e => c.SetMusic(e.newValue));
            view.Q<Slider>("sfx-slider")?.RegisterValueChangedCallback(e => c.SetSfx(e.newValue));
            view.Q<RadioButtonGroup>("quality-radio-group")?.RegisterValueChangedCallback(e => c.SetQuality(e.newValue));
            view.Q<Slider>("sensitivity-slider")?.RegisterValueChangedCallback(e => c.SetSensitivity(e.newValue));
            
            view.Q<Button>("settings-exit-btn")?.RegisterCallback<ClickEvent>(_ => c.Back());
        }

        public static void BindAbout(VisualElement root, CreditsController c) {
            VisualElement view = root.Q("About");
            if (view == null) return;
            
            view.Q<Button>("credits-exit-btn")?.RegisterCallback<ClickEvent>(_ => c.Back());
            
        }
    }
}