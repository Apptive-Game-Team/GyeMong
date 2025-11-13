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
        Sandworm = 8,
    }
    
    public static class StageSelectPage
    {
        public static readonly string MAX_STAGE_ID_KEY = "MaxStageId";
        private const string SceneName = "StageSelectPage";

        public static void LoadStageSelectPage()
        {
            SceneLoader.LoadScene(SceneName);
        }
        public static void LoadStageSelectPage(int maxStageId)
        {
            PlayerPrefs.SetInt("MaxStageId", maxStageId);
            SceneLoader.LoadScene(SceneName);
        }
        
        public static void LoadStageSelectPageOnStage(int currentStageId)
        {
            PlayerPrefs.SetInt("CurrentStageId", currentStageId);
            SceneLoader.LoadScene(SceneName);
        }
        
        public static void LoadStageSelectPageOnStageToDestination(Stage currentStageId, Stage maxStageId)
        {
            Debug.Log("currentStageId: " + currentStageId+ " maxStageId: " + maxStageId);;
            PlayerPrefs.SetInt("CurrentStageId", (int) currentStageId);
            PlayerPrefs.SetInt(MAX_STAGE_ID_KEY, (int) maxStageId);
            Debug.Log("CurrentStageId: " + PlayerPrefs.GetInt("CurrentStageId", 0) + " MaxStageId: " + PlayerPrefs.GetInt(MAX_STAGE_ID_KEY, 1));
            SceneLoader.LoadScene(SceneName);
        }
    }
}