using CocaCopa.SceneManagement;
using UnityEngine;
using HoldMyBeer.Input;
    
namespace HoldMyBeer.UI {
    public class GameFlowController {
        private readonly UINavigation nav;
        public bool IsPaused {get; private set;}

        public GameFlowController(UINavigation nav) {
            this.nav = nav;

            PlayerInput.Instance.OnEscKeyPressed += OnPaused;
        }

        private void OnPaused() {
            TogglePause();
        }

        public void Endgame() {
            nav.Open(UIScreen.Endgame);
        }

        private void TogglePause() {
            if (IsPaused) { ResumeGame();}
            else { PauseGame();}
            ToggleCursor();
        }
        
        private void ToggleCursor() {
            Cursor.visible = IsPaused;
            Cursor.lockState = IsPaused ? CursorLockMode.None : CursorLockMode.Locked;
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

        public void Restart() {
            Time.timeScale = 1f;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.UI);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.Player);
            SceneTransitionApi.TransitionToScene(2, LoadMode.Single);
        }

        public void MainMenu() {
            Time.timeScale = 1f;
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.Player);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
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