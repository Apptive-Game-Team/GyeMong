using System.Collections;
using Cinemachine;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Map.Portal;
using GyeMong.GameSystem.Map.Stage.ScriptableObject;
using GyeMong.GameSystem.Map.Stage.Select;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Stage
{
    public class StageManager
    {
        private static StageInfo _currentStageInfo;

        public static void EnterStage(StageInfo stageInfo, MonoBehaviour context)
        {
            _currentStageInfo = stageInfo;
            context.StartCoroutine(EnterStageCoroutine());
        }
        
        public static void ClearStage(MonoBehaviour context)
        {
            if (_currentStageInfo == null)
            {
                Debug.LogError("No stage is currently loaded.");
                return;
            } 
            SceneContext.Character.gameObject.SetActive(false);
            context.StartCoroutine(ClearStageCoroutine());
        }
        
        public static void LoseStage(MonoBehaviour context)
        {
            if (_currentStageInfo == null)
            {
                Debug.LogError("No stage is currently loaded.");
                return;
            } 
            SceneContext.Character.gameObject.SetActive(false);
            StageSelectPage.LoadStageSelectPageOnStage((int) _currentStageInfo.id);
        }

        private static IEnumerator EnterStageCoroutine()
        {
            if (_currentStageInfo.beforeScript != null)
            {
                foreach (var script in _currentStageInfo.beforeScript)
                {
                    yield return script.Play();
                }
            }
            PortalManager.Instance.LoadSceneMode(_currentStageInfo.portalID);
        }

        private static IEnumerator ClearStageCoroutine()
        {
            if (_currentStageInfo.afterScript != null)
            {
                foreach (var script in _currentStageInfo.afterScript)
                {
                    yield return script.Play();
                }
            }
            
            StageSelectPage.LoadStageSelectPageOnStageToDestination(_currentStageInfo.id, _currentStageInfo.id + 1);
        }
    }
}