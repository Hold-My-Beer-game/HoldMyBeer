using UnityEngine;
using UnityEngine.InputSystem;
using System;

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
        public event Action OnEscKeyPressed;

        public enum InputMap {
            Player, UI, Gameplay
        }

        private void Awake() {
            Init();
        }

        private void Init() {
            Actions = new InputSystem_Actions();
            Actions.Enable();
            KeepAlive();
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
        private void OnPause(InputValue value) {
            if (value.isPressed) {
                OnEscKeyPressed?.Invoke();
            }
        }

        public void EnableInputMap(InputMap inputMap) {
            switch (inputMap) {
                case InputMap.Player: if (!Actions.Player.enabled) { Actions.Player.Enable(); } break;
                case InputMap.UI: if (!Actions.UI.enabled) { Actions.UI.Enable(); } break;
                case InputMap.Gameplay: if (!Actions.Gameplay.enabled) { Actions.Gameplay.Enable(); } break;
            }
            Debug.Log($"Enable Input Map {inputMap}");
            Debug.Log($"Cursor.visible = {Cursor.visible} || Cursor.lockState = {Cursor.lockState} ");
        }

        public void DisableInputMap(InputMap inputMap) {
            switch (inputMap) {
                case InputMap.Player: if (Actions.Player.enabled) { Actions.Player.Disable(); } break;
                case InputMap.UI: if (Actions.UI.enabled) { Actions.UI.Disable(); } break;
                case InputMap.Gameplay: if (Actions.Gameplay.enabled) { Actions.Gameplay.Disable(); } break;
            }
            Debug.Log($"Disable Input Map {inputMap}");
        }
    }
}