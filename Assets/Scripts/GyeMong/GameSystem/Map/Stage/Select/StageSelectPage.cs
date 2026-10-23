using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace GyeMong.GameSystem.Map.Stage.Select
{
    public enum Stage
    {
        Beach = 0,
        Slime = 1,
        Elf = 2,
        Shadow = 3,
        Golem = 4,
        NagaRouge = 5,
        NagaWarrior = 6,
        Wanderer = 7,
    }
    
    public static class StageSelectPage
    {
        public static readonly string MAX_STAGE_ID_KEY = "MaxStageId";
        private const string SceneName = "StageSelectPage";

        public static void SaveProgress(int unlockedStageId)
        {
            int previous = PlayerPrefs.GetInt(MAX_STAGE_ID_KEY, 0);
            PlayerPrefs.SetInt(MAX_STAGE_ID_KEY, Mathf.Clamp(Mathf.Max(previous, unlockedStageId), 0, (int)Stage.Wanderer));
        }

        public static void LoadStageSelectPage()
        {
            SceneLoader.LoadScene(SceneName);
        }
        public static void LoadStageSelectPage(int maxStageId)
        {
            SaveProgress(maxStageId);
            SceneLoader.LoadScene(SceneName);
        }
        
        public static void LoadStageSelectPageOnStage(int currentStageId)
        {
            PlayerPrefs.SetInt("CurrentStageId", Mathf.Clamp(currentStageId, 0, (int)Stage.Wanderer));
            SceneLoader.LoadScene(SceneName);
        }
        
        public static void LoadStageSelectPageOnStageToDestination(Stage currentStageId, Stage maxStageId)
        {
            PlayerPrefs.SetInt("CurrentStageId", Mathf.Clamp((int)currentStageId, 0, (int)Stage.Wanderer));
            SaveProgress((int)maxStageId);
            Debug.Log("CurrentStageId: " + PlayerPrefs.GetInt("CurrentStageId", 0) + " MaxStageId: " + PlayerPrefs.GetInt(MAX_STAGE_ID_KEY, 1));
            SceneLoader.LoadScene(SceneName);
        }
    }
}