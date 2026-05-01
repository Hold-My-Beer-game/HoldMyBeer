using System;

namespace CocaCopa.SceneManagement {
    public interface ISceneTransition {
        event Action OnSceneLoaded;
        event Action OnSceneTransitionStarted;
        event Action OnSceneTransitionCompleted;

        bool TryAddExternalLoadingFlag(out int callerId);
        bool TryCompleteExternalLoadingFlag(int callerId);
        void TransitionToScene(int sceneIndex, LoadMode loadMode, ILoadingScreen loadingScreen = null);
    }
}