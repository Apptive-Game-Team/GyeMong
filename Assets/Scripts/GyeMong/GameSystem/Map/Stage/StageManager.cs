using System.Collections;
using GyeMong.GameSystem.Map.Portal;
using GyeMong.GameSystem.Map.Stage.ScriptableObject;
using GyeMong.GameSystem.Map.Stage.Select;
using UnityEngine;
using Util;
using GyeMong.EventSystem.Controller;
using UnityEngine.SceneManagement;

namespace GyeMong.GameSystem.Map.Stage
{
    public class StageManager
    {
        private static StageInfo _currentStageInfo;
        private static bool _clearing;
        private static bool _entering;
        private static Coroutine enterRoutine;
        private static Coroutine clearRoutine;

        public static void EnterStage(StageInfo stageInfo, MonoBehaviour context)
        {
            if (_entering)
            {
                Debug.LogError("Enter stage coroutine is already running.");
                return;
            }
            
            if (stageInfo == null || context == null || SceneLoader.Instance.IsLoading) return;
            _currentStageInfo = stageInfo;
            _entering = true;
            var routine = SceneLoader.Instance.StartCoroutine(EnterStageCoroutine(stageInfo));
            if (_entering) enterRoutine = routine;
        }
        
        public static void ClearStage(MonoBehaviour context)
        {
            if (_currentStageInfo == null)
            {
                Debug.LogError("No stage is currently loaded.");
                return;
            } 
            if (_clearing)
            {
                Debug.LogWarning("Clear stage coroutine is already running.");
                return;
            }
            if (SceneContext.Character != null) SceneContext.Character.gameObject.SetActive(false);
            _clearing = true;
            var routine = SceneLoader.Instance.StartCoroutine(ClearStageCoroutine(_currentStageInfo));
            if (_clearing) clearRoutine = routine;
        }
        
        public static void LoseStage(MonoBehaviour context)
        {
            if (_currentStageInfo == null)
            {
                Debug.LogError("No stage is currently loaded.");
                return;
            } 
            if (SceneContext.Character != null) SceneContext.Character.gameObject.SetActive(false);
            StageSelectPage.LoadStageSelectPageOnStage((int) _currentStageInfo.id);
        }

        public static void CancelTransitions(SceneLoader owner)
        {
            var entering = enterRoutine;
            var clearing = clearRoutine;
            enterRoutine = clearRoutine = null;
            _entering = _clearing = false;
            if (entering != null) owner.StopCoroutine(entering);
            if (clearing != null) owner.StopCoroutine(clearing);
            ChatController.StopDialogueSounds();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetRuntime()
        {
            _currentStageInfo = null;
            _entering = false;
            _clearing = false;
            enterRoutine = clearRoutine = null;
        }

        private static IEnumerator EnterStageCoroutine(StageInfo stage)
        {
            int sourceScene = SceneManager.GetActiveScene().handle;
            try
            {
                if (stage.beforeScript != null)
                    foreach (var script in stage.beforeScript)
                    {
                        if (script != null) yield return script.Play();
                        if (SceneManager.GetActiveScene().handle != sourceScene || SceneLoader.Instance.IsLoading) yield break;
                    }
                _entering = false;
                enterRoutine = null;
                PortalManager.Instance.LoadSceneMode(stage.portalID);
            }
            finally { _entering = false; enterRoutine = null; }
        }

        private static IEnumerator ClearStageCoroutine(StageInfo stage)
        {
            int sourceScene = SceneManager.GetActiveScene().handle;
            try
            {
                if (stage.afterScript != null)
                    foreach (var script in stage.afterScript)
                    {
                        if (script != null) yield return script.Play();
                        if (SceneManager.GetActiveScene().handle != sourceScene || SceneLoader.Instance.IsLoading) yield break;
                    }
                _clearing = false;
                clearRoutine = null;
                StageSelectPage.LoadStageSelectPageOnStageToDestination(stage.id, stage.id + 1);
            }
            finally { _clearing = false; clearRoutine = null; }
        }
    }
}
