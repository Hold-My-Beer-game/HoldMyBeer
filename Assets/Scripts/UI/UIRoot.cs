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

        private GameFlowController gameFlow;
        internal HUDState HUDState => hudState;

        private void Awake() {
            registry.Init();

            nav = new UINavigation(registry.Map, startScreen);

            VisualElement root = registry.Root;

            SettingsController settings = new SettingsController(nav);

            gameFlow = new GameFlowController(nav);

            if (startScreen == UIScreen.MainMenu) {
                MainMenuController menu = new MainMenuController(nav);
                CreditsController credits = new CreditsController(nav);

                UIBinding.BindMainMenu(root, menu);
                UIBinding.BindSettings(root, settings);
                UIBinding.BindAbout(root, credits);
            }
            else if (startScreen == UIScreen.Endgame) {
                UIBinding.BindPause(root, gameFlow);
                UIBinding.BindEndgame(root, gameFlow);
            }
            else {
                hudState = new HUDState();
                playerReadState = player.GetComponent<IPlayerStateRead>();

                HUDController hud = new HUDController(hudState, playerReadState);

                hud.Init();
                hudView.Bind(hudState);
                if (tester != null) { tester.SetState(hudState); }

                UIBinding.BindPause(root, gameFlow);
                UIBinding.BindEndgame(root, gameFlow);
                UIBinding.BindSettings(root, settings);
            }
        }
    }
}