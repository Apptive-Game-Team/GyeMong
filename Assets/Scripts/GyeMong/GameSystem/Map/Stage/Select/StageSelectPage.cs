using UnityEngine;
using UnityEngine.SceneManagement;

namespace GyeMong.GameSystem.Map.Stage.Select
{
    public enum Stage
    {
        Beach = 0,
        Slime = 1,
        Elf = 2,
        Shadow = 3,
        Golem = 4,
    }
    
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
        
        public static void LoadStageSelectPageOnStage(int currentStageId)
        {
            PlayerPrefs.SetInt("CurrentStageId", currentStageId);
            SceneManager.LoadScene(SceneName);
        }
        
        public static void LoadStageSelectPageOnStageToDestination(Stage currentStageId, Stage maxStageId)
        {
            PlayerPrefs.SetInt("CurrentStageId", (int) currentStageId);
            PlayerPrefs.SetInt("MaxStageId", (int) maxStageId);
            SceneManager.LoadScene(SceneName);
        }
    }
}