using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public class SceneLoader : SingletonObject<SceneLoader>
    {
        public bool IsLoading { get; private set; }
        public static void LoadScene(string sceneName, int delay = 2)
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Scene is not included in the build: {sceneName}");
                return;
            }
            if (Instance == null || Instance.IsLoading) return;
            GyeMong.GameSystem.Map.Stage.StageManager.CancelTransitions(Instance);
            Instance.IsLoading = true;
            Instance.StartCoroutine(Instance.LoadSceneCoroutine(sceneName, delay));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, int delay)
        {
            GyeMong.UISystem.Option.OptionUIToggler.Instance?.CloseWindow();
            SceneTransitionCurtain.Cover();
            GyeMong.GameSystem.GameTime.Scale = 1f;
            try
            {
                if (delay > 0)
                {
                    yield return SceneManager.LoadSceneAsync("LoadingScene");
                    yield return new WaitForSecondsRealtime(delay);
                }
                yield return SceneManager.LoadSceneAsync(sceneName);
                SceneTransitionCurtain.RevealWhenReady();
                yield return new WaitWhile(() => SceneTransitionCurtain.IsCovered);
            }
            finally
            {
                if (SceneTransitionCurtain.IsCovered) SceneTransitionCurtain.RevealWhenReady();
                IsLoading = false;
            }
        }
    }
}
