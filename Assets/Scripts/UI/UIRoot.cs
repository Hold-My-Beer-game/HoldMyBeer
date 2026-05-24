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
        [SerializeField] private Transform player;

        private UINavigation nav;
        private IPlayerStateRead playerReadState;

        private HUDState hudState;
        internal HUDState HUDState => hudState;

        private void Awake() {
            registry.Init();

            nav = new UINavigation(registry.Map, startScreen);

            VisualElement root = registry.Root;
            
            // settingsReadState = settingsState
            SettingsController settings = new SettingsController(nav);

            if (startScreen == UIScreen.MainMenu) {
                MainMenuController menu = new MainMenuController(nav);
                CreditsController credits = new CreditsController(nav);

                UIBinding.BindMainMenu(root, menu);
                UIBinding.BindSettings(root, settings);
                UIBinding.BindAbout(root, credits);
            }
            else if (startScreen == UIScreen.Endgame) {
                GameFlowController flow = new GameFlowController(nav);
                UIBinding.BindPause(root, flow);
                UIBinding.BindEndgame(root, flow);
                
            }
            else {
                GameFlowController flow = new GameFlowController(nav);

                hudState = new HUDState();
                playerReadState = player.GetComponent<IPlayerStateRead>();
                HUDController hud = new HUDController(hudState, playerReadState);
                hud.Init();
                hudView.Bind(hudState);
                if (tester != null) { tester.SetState(hudState); }

                UIBinding.BindPause(root, flow);
                UIBinding.BindEndgame(root, flow);
                UIBinding.BindSettings(root, settings);
            }
        }
    }
}