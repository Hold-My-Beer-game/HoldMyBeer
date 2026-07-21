using HoldMyBeer.Audio;
using HoldMyBeer.Input;
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
        [SerializeField] private Transform player;

        private UINavigation nav;
        private VisualElement root;
        private IPlayerStateRead playerReadState;

        private HUDState hudState;

        private GameFlowController gameFlow;

        private HUDController hud;
        internal HUDState HUDState => hudState;

        private void Awake() {
            registry.Init();

            nav = new UINavigation(registry.Map, startScreen);

            root = registry.Root;
           
            if (startScreen == UIScreen.MainMenu) {
                PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
                PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.Player);

                MainMenuController menu = new MainMenuController(nav);
                CreditsController credits = new CreditsController(nav);

                UIBinding.BindMainMenu(root, menu);
                UIBinding.BindAbout(root, credits);
            }
            else {
                gameFlow = new GameFlowController(nav);
                PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.UI);
                PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.Player);
                
                hudState = new HUDState();
                playerReadState = player.GetComponent<IPlayerStateRead>();

                hud = new HUDController(hudState, playerReadState, gameFlow);

                hud.Init();
                hudView.Bind(hudState);

                hudView.ShowControls();
                
                UIBinding.BindPause(root, gameFlow);
                UIBinding.BindEndgame(root, gameFlow);
            }
        }

        private void Start() {
            SettingsController settings = new SettingsController(nav);
            UIBinding.BindSettings(root, settings);
            settings.SetMaster(AudioManager.instance.MasterVolume);
            settings.SetMusic(AudioManager.instance.MusicVolume);
            settings.SetSfx(AudioManager.instance.SFXVolume);
        }

        private void OnDestroy() {
            gameFlow?.Dispose();
            hud?.Dispose();
        }
        
    }
}