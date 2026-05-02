using UnityEngine;

namespace CocaCopa.SceneManagement {
    public static class SceneTransitionBootstrap {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init() {
            var sceneTransition = new SceneTransition(40);
            SceneTransitionApi.Bind(sceneTransition);
        }
    }
}