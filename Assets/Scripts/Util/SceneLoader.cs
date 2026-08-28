using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public class SceneLoader : SingletonObject<SceneLoader>
    {
        private const float DEFAULT_MIN_LOADING_TIME = 2f;

        public static void LoadScene(string sceneName, float minLoadingTime = DEFAULT_MIN_LOADING_TIME)
        {
            Instance.StartCoroutine(LoadSceneRoutine(sceneName, minLoadingTime));
        }

        /// <summary>
        /// 로딩 씬을 거쳐 대상 씬을 띄우고, 대상 씬이 실제로 활성화될 때까지 대기한다.
        /// 호출부가 yield return 하면 씬 준비가 끝난 뒤에 다음 줄이 실행된다.
        /// </summary>
        public static IEnumerator LoadSceneRoutine(string sceneName, float minLoadingTime = DEFAULT_MIN_LOADING_TIME)
        {
            SceneManager.LoadScene("LoadingScene");
            yield return null; // 로딩 씬이 실제로 올라온 다음 프레임부터 진행

            float startTime = Time.unscaledTime;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            // allowSceneActivation 이 false 인 동안 progress 는 0.9 에서 멈춘다.
            while (operation.progress < 0.9f)
            {
                yield return null;
            }

            float remaining = minLoadingTime - (Time.unscaledTime - startTime);
            if (remaining > 0f)
            {
                yield return new WaitForSecondsRealtime(remaining);
            }

            operation.allowSceneActivation = true;
            while (!operation.isDone)
            {
                yield return null;
            }

            yield return null; // 새 씬의 Awake/Start 가 끝난 뒤 반환
        }
    }
}
