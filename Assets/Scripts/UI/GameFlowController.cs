using CocaCopa.SceneManagement;
using UnityEngine;
using HoldMyBeer.Input;

namespace HoldMyBeer.UI {
    public class GameFlowController {
        private readonly UINavigation nav;
        
        private bool IsPaused {get; set;}
        

        public GameFlowController(UINavigation nav) {
            this.nav = nav;
            PlayerInput.Instance.OnTabKeyPressed += OnPaused;
        }
        
        private void OnPaused() {
            TogglePause();
        }

        public void Endgame() {
            IsPaused = true;
            Time.timeScale = 0;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.Player);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
            
            nav.Open(UIScreen.Endgame);
        }

        private void TogglePause() {
            if (IsPaused) { ResumeGame();}
            else { PauseGame();}
        }
        
        private void PauseGame() {
            if (IsPaused) { return;}
            IsPaused = true;
            Time.timeScale = 0;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.Player);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
            nav.Open(UIScreen.Pause);
            Debug.Log($"TimeScale: {Time.timeScale}");
        }

        internal void ResumeGame() {
            if (!IsPaused) { return; }
            IsPaused = false;
            Time.timeScale = 1f;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.UI);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.Player);
            nav.Back();
            Debug.Log($"TimeScale: {Time.timeScale}");
        }

        public static void Restart() {
            Time.timeScale = 1f;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.UI);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.Player);
            SceneTransitionApi.TransitionToScene(2, LoadMode.Single);
        }

        public static void MainMenu() {
            Time.timeScale = 1f;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.Player);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
            SceneTransitionApi.TransitionToScene(0, LoadMode.Single);
        }

        public void Settings() {
            nav.Open(UIScreen.Settings);
        }

        public static void Quit() {
            Application.Quit();
        }
    }
}