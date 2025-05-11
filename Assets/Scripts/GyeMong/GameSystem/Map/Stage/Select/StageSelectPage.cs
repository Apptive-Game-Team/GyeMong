using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map.Stage.Select
{
    public static class StageSelectPage
    {
        private const string SceneName = "StageSelectPage";

        public static void LoadStageSelectPage()
        {
            SceneManager.LoadScene(SceneName);
        }
        public static void LoadStageSelectPage(int maxStageId)
        {
            PlayerPrefs.SetInt("MaxStageId", maxStageId);
            SceneManager.LoadScene(SceneName);
        }
    }
}