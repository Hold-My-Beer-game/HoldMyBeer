using UnityEngine;
using UnityEngine.UIElements;

namespace HoldMyBeer.UI {
    /// <summary>
    /// Scene-local UI composition root.
    /// </summary>
    public class UIRoot : MonoBehaviour {
        [SerializeField] private UIScreenRegistry registry;
        [SerializeField] private UIScreen startScreen;
        [SerializeField] private HUDView hudView;
        [SerializeField] private HUDTester tester;

        private UINavigation nav;
        
        private HUDState hudState;
        internal HUDState HUDState => hudState;

        private void Awake() {
            nav = new UINavigation(registry.Map, startScreen);

            VisualElement root = registry.Root;

            // Scene-specific wiring

            if (startScreen == UIScreen.MainMenu) {
                MainMenuController menu = new MainMenuController(nav);
                SettingsState settingsState = new SettingsState();
                SettingsController settings = new SettingsController(settingsState, nav);
                CreditsController credits = new CreditsController(nav);

                UIBinding.BindMainMenu(root, menu);
                UIBinding.BindSettings(root, settings);
                UIBinding.BindAbout(root, credits);
            }
            else {
                SettingsState settingsState = new SettingsState();
                SettingsController settings = new SettingsController(settingsState, nav);

                GameFlowController flow = new GameFlowController(nav);

                hudState = new HUDState();
                HUDController hud = new HUDController(hudState);
                hudView.Bind(hudState);
                if (tester != null) { tester.SetState(hudState); }
                
                UIBinding.BindPause(root, flow);
                UIBinding.BindEndgame(root, flow);
                UIBinding.BindSettings(root, settings);
                
            }
        }
    }
}