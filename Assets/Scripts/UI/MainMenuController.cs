using UnityEngine;
using CocaCopa.SceneManagement;
using HoldMyBeer.Input;

namespace HoldMyBeer.UI {
    public class MainMenuController {
        private readonly UINavigation nav;

        public MainMenuController(UINavigation nav) {
            this.nav = nav;
        }

        public void StartGame() {
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.Player);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
            SceneTransitionApi.TransitionToScene(1, LoadMode.Single);
        }

        public void OpenSettings() {
            nav.Open(UIScreen.Settings);
        }

        public void OpenAbout() {
            nav.Open(UIScreen.About);
        }

        public void Quit() {
            Application.Quit();
        }
    }
}