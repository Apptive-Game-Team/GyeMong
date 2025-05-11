using System.Collections;

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
            context.StartCoroutine(ClearStageCoroutine());
        }
        
        private static IEnumerator EnterStageCoroutine()
        {
            if (_currentStageInfo.beforeScript != null)
            {
                yield return _currentStageInfo.beforeScript.Execute();
            }
            PortalManager.Instance.LoadSceneMode(_currentStageInfo.portalID);
        }

        private static IEnumerator ClearStageCoroutine()
        {
            if (_currentStageInfo.afterScript != null)
            {
                yield return _currentStageInfo.afterScript.Execute();
            }
            StageSelectPage.LoadStageSelectPageOnStageToDestination(_currentStageInfo.id, _currentStageInfo.id + 1);
        }
    }
}