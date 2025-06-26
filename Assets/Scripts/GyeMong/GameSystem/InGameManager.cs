using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GyeMong.GameSystem
{
    public class InGameManager : MonoBehaviour
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
