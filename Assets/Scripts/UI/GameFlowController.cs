using CocaCopa.SceneManagement;
using UnityEngine;

namespace HoldMyBeer.UI {
    public class GameFlowController {
        private readonly UINavigation nav;

        public GameFlowController(UINavigation nav) {
            this.nav = nav;
        }

        public void Endgame() {
            nav.Open(UIScreen.Endgame);
        }

        public void PauseGame() {
            nav.Open(UIScreen.Pause);
        }

        public void ResumeGame() {
            nav.Back();
        }

        public void Restart() {
            SceneTransitionApi.TransitionToScene(2, LoadMode.Single);
        }

        public void MainMenu() {
            SceneTransitionApi.TransitionToScene(0, LoadMode.Single);
        }

        public void Settings() {
            nav.Open(UIScreen.Settings);
        }

        public void Quit() {
            Application.Quit();
        }
    }
}