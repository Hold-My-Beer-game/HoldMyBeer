using UnityEngine;
using CocaCopa.SceneManagement;

namespace HoldMyBeer.UI {
    public class MainMenuController {
        private readonly UINavigation nav;

        public MainMenuController(UINavigation nav) {
            this.nav = nav;
        }

        public void StartGame() {
            // TODO: call IAppActions.StartGame()
            SceneTransitionApi.TransitionToScene(1, LoadMode.Single);
        }

        public void OpenSettings() {
            nav.Open(UIScreen.Settings);
        }

        public void OpenAbout() {
            nav.Open(UIScreen.About);
        }

        public void Quit() {
            // TODO: call IAppActions.Quit()
            Application.Quit();
        }
    }
}