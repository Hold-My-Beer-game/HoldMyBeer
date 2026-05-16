using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CocaCopa.SceneManagement {
    public sealed class SceneTransition : ISceneTransition {
        private readonly float timeout;
        private readonly Dictionary<int, bool> externalFlags = new();

        public event Action OnSceneLoaded;
        public event Action OnSceneTransitionStarted;
        public event Action OnSceneTransitionCompleted;

        private bool isLoadingScene;
        private bool canAcceptExternalFlags;
        private int id;

        internal SceneTransition(float timeout) {
            this.timeout = timeout;
        }

        /// <summary>
        /// Registers an external loading flag for the given caller if allowed.
        /// Call this in Awake/OnEnable to ensure the caller blocks the completion of the loading phase.
        /// </summary>
        /// <param name="callerId"></param>
        /// <returns></returns>
        public bool TryAddExternalLoadingFlag(out int callerId) {
            if (!canAcceptExternalFlags) {
                Debug.LogWarning($"[{nameof(SceneTransition)}] Time window for accepting external loading flags had been closed.");
                callerId = -1;
                return false;
            }
            callerId = ++id;
            externalFlags[callerId] = false;
            return true;
        }

        /// <summary>
        /// Marks the previously registered external loading flag for the given caller as complete.
        /// Once all flags are complete, the loading phase will be completed.
        /// </summary>
        /// <param name="callerId"></param>
        /// <returns></returns>
        public bool TryCompleteExternalLoadingFlag(int callerId) {
            if (externalFlags.ContainsKey(callerId)) {
                externalFlags[callerId] = true;
                return true;
            }
            Debug.LogWarning($"{nameof(SceneTransition)}] Could not find an external loading flag with the given id.");
            return false;
        }

        /// <summary>
        /// Starts loading the given scene, handling loading flow and waiting for external operations.
        /// </summary>
        /// <param name="sceneIndex"></param>
        /// <param name="loadingScreen"></param>
        public void TransitionToScene(int sceneIndex, LoadMode loadMode, ILoadingScreen loadingScreen = null) {
            if (isLoadingScene) {
                Debug.LogWarning($"[{nameof(SceneTransition)}] Already loading a scene.");
                return;
            }
            _ = TransitionToSceneAsync(sceneIndex, loadMode, loadingScreen);
        }

        private async Task TransitionToSceneAsync(int sceneIndex, LoadMode loadMode, ILoadingScreen loadingScreen) {
            externalFlags.Clear();
            OnSceneTransitionStarted?.Invoke();
            isLoadingScene = true;
            canAcceptExternalFlags = true;

            if (loadingScreen != null) { await loadingScreen.Show(); }
            await LoadSceneAsync(sceneIndex, SceneModeToUnity(loadMode));
            OnSceneLoaded?.Invoke();
            await Task.Yield();
            canAcceptExternalFlags = false;
            await ExternalOperations();
            if (loadingScreen != null) { await loadingScreen.Hide(); }
            isLoadingScene = false;
            OnSceneTransitionCompleted?.Invoke();
        }

        private static async Task LoadSceneAsync(int sceneIndex, LoadSceneMode loadSceneMode) {
            AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
            if (sceneLoad is null) { throw new Exception($"[{nameof(SceneTransition)}] Something went wrong when loading the scene"); }
            while (!sceneLoad.isDone) { await Task.Yield(); }
        }

        private static LoadSceneMode SceneModeToUnity(LoadMode loadMode) {
            return loadMode switch
            {
                LoadMode.Single => LoadSceneMode.Single,
                _ => LoadSceneMode.Single
            };
        }

        private async Task ExternalOperations() {
            float timeRemaining = timeout;
            if (externalFlags.Count == 0) { return; }

            while (true) {
                bool allCompleted = true;
                foreach (KeyValuePair<int, bool> kvp in externalFlags) {
                    if (kvp.Value) { continue; }
                    allCompleted = false;
                    break;
                }
                if (allCompleted) { break; }

                timeRemaining -= Time.unscaledDeltaTime;
                if (timeRemaining <= 0f) {
                    foreach (KeyValuePair<int, bool> kvp in externalFlags) {
                        if (!kvp.Value) {
                            Debug.LogError(
                                $"[{nameof(SceneTransition)}] Timeout waiting for id: {kvp.Key}"
                            );
                        }
                    }
                    break;
                }
                await Task.Yield();
            }
        }
    }
}