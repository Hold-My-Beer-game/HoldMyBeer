using UnityEngine;
using UnityEngine.InputSystem;
using System;
using CocaCopa.Logger.API;

namespace HoldMyBeer.Input {
    public class PlayerInput : MonoBehaviour {
        private static PlayerInput instance;
        
        public static PlayerInput Instance {
            get {
                 if (instance == null) {
                     var inputObj = new GameObject("PlayerInput (Lazy Initialized)");
                     var customPlayerInput = inputObj.AddComponent<HoldMyBeer.Input.PlayerInput>();
                     var unityPlayerInput = inputObj.AddComponent<UnityEngine.InputSystem.PlayerInput>();
                     unityPlayerInput.actions = customPlayerInput.Actions.asset;
                 }
                 return instance;
            }   
        }

        public InputSystem_Actions Actions { get; private set; }

        public event Action OnJumpKeyPressed;
        public event Action OnTabKeyPressed;

        public enum InputMap {
            Player, UI, Gameplay
        }

        private void Awake() {
            KeepAlive();
            Init();
            Actions.Gameplay.Pause.performed += _ => {OnTabKeyPressed?.Invoke(); Log.Info("TAB pressed", LogColor.Orange); };
        }

        private void Init() {
            Actions = new InputSystem_Actions();
            Actions.Enable();
        }
        
        private void KeepAlive() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this) {
                Destroy(gameObject);
            }
        }
        private void OnJump(InputValue value) {
            if (value.isPressed) {
                OnJumpKeyPressed?.Invoke();
            }
        }
        // private void OnPause(InputValue value) {
        //     if (value.isPressed) {
        //         OnTabKeyPressed?.Invoke();
        //         Log.Info("TAB key pressed", LogColor.Orange);
        //     }
        // }

        public void EnableInputMap(InputMap inputMap) {
            switch (inputMap) {
                case InputMap.Player: if (!Actions.Player.enabled) { Actions.Player.Enable(); } ToggleCursor(false); break;
                case InputMap.UI: if (!Actions.UI.enabled) { Actions.UI.Enable(); } ToggleCursor(true); break;
                case InputMap.Gameplay: if (!Actions.Gameplay.enabled) { Actions.Gameplay.Enable(); } break;
            }
            
        }

        public void DisableInputMap(InputMap inputMap) {
            switch (inputMap) {
                case InputMap.Player: if (Actions.Player.enabled) { Actions.Player.Disable(); } break;
                case InputMap.UI: if (Actions.UI.enabled) { Actions.UI.Disable(); } break;
                case InputMap.Gameplay: if (Actions.Gameplay.enabled) { Actions.Gameplay.Disable(); } break;
            }
        }
        
        public static void ToggleCursor(bool visible) {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}