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
        private static Coroutine _clearStageCoroutine;
        private static Coroutine _enterStageCoroutine;

        public static void EnterStage(StageInfo stageInfo, MonoBehaviour context)
        {
            if (_enterStageCoroutine != null)
            {
                Debug.LogError("Enter stage coroutine is already running.");
                return;
            }
            
            _currentStageInfo = stageInfo;
            _enterStageCoroutine = context.StartCoroutine(EnterStageCoroutine());
        }
        
        public static void ClearStage(MonoBehaviour context)
        {
            if (_currentStageInfo == null)
            {
                Debug.LogError("No stage is currently loaded.");
                return;
            } 
            if (_clearStageCoroutine != null)
            {
                Debug.LogError("Clear stage coroutine is already running.");
            }
            SceneContext.Character.gameObject.SetActive(false);
            _clearStageCoroutine = context.StartCoroutine(ClearStageCoroutine());
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
            _enterStageCoroutine = null;
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
            _clearStageCoroutine = null;
        }
    }
}