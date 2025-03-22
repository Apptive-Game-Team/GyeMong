using UnityEngine.SceneManagement;
using Util;

namespace System.Game
{
    public class InGameManager : SingletonObject<InGameManager>
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "TitleScene")
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
