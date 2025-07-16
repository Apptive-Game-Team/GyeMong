using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Util
{
    public class SceneLoader : SingletonObject<SceneLoader>
    {
        public static void LoadScene(string sceneName, int delay = 2)
        {
            Instance.StartCoroutine(LoadSceneCoroutine(sceneName, delay));
        }

        private static IEnumerator LoadSceneCoroutine(string sceneName, int delay)
        {
            SceneManager.LoadScene("LoadingScene");
            yield return new WaitForSeconds(delay);
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
