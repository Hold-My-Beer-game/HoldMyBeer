using System;

namespace CocaCopa.SceneManagement {
    public static class SceneTransitionApi {
        private static ISceneTransition sceneTransition;

        internal static void Bind(ISceneTransition sceneTransitionRef) {
            sceneTransition = sceneTransitionRef;
        }

        public static event Action OnSceneTransitionStarted {
            add => sceneTransition.OnSceneTransitionStarted += value;
            remove => sceneTransition.OnSceneTransitionStarted -= value;
        }

        public static event Action OnSceneLoaded {
            add => sceneTransition.OnSceneLoaded += value;
            remove => sceneTransition.OnSceneLoaded -= value;
        }

        public static event Action OnSceneTransitionCompleted {
            add => sceneTransition.OnSceneTransitionCompleted += value;
            remove => sceneTransition.OnSceneTransitionCompleted -= value;
        }

        public static void TransitionToScene(int sceneIndex, LoadMode loadMode, ILoadingScreen loadingScreen = null) {
            sceneTransition.TransitionToScene(sceneIndex, loadMode, loadingScreen);
        }

        public static bool TryAddExternalLoadingFlag(out int callerId) {
            return sceneTransition.TryAddExternalLoadingFlag(out callerId);
        }

        public static bool TryComplete(int callerId) {
            return sceneTransition.TryCompleteExternalLoadingFlag(callerId);
        }
    }
}