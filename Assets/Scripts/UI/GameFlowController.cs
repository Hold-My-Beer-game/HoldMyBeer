using System;
using CocaCopa.Logger.API;
using CocaCopa.SceneManagement;
using UnityEngine;
using HoldMyBeer.Input;

namespace HoldMyBeer.UI {
    public class GameFlowController : IDisposable {
        private readonly UINavigation nav;
        private bool IsPaused {get; set;}
        
        public GameFlowController(UINavigation nav) {
            this.nav = nav;
            PlayerInput.Instance.OnTabKeyPressed += TogglePause;
        }
        
        public void Dispose() {
            if (PlayerInput.Instance == null) return;
            PlayerInput.Instance.OnTabKeyPressed -= TogglePause;
        }

        public void Endgame() {
            Time.timeScale = 0;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.Player);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
            
            nav.Open(UIScreen.Endgame);
        }

        private void TogglePause() {
            if (IsPaused) { ResumeGame();}
            else { PauseGame();}
            Log.Info($"Pause toggled: {IsPaused}", LogColor.Magenta);
        }
        
        private void PauseGame() {
            if (IsPaused) { return;}
            IsPaused = true;
            Time.timeScale = 0;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.Player);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.UI);
            Log.Info("Game paused", LogColor.Yellow);
            nav.Open(UIScreen.Pause);
        }

        internal void ResumeGame() {
            if (!IsPaused) { return; }
            IsPaused = false;
            Time.timeScale = 1f;
            PlayerInput.Instance.DisableInputMap(PlayerInput.InputMap.UI);
            PlayerInput.Instance.EnableInputMap(PlayerInput.InputMap.Player);
            Log.Info("Game resumed", LogColor.Blue);
            nav.Back();
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