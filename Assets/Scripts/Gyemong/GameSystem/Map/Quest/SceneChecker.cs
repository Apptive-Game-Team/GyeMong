using Gyemong.EventSystem;
using Gyemong.EventSystem.Controller.Condition;
using UnityEngine;
using UnityEngine.SceneManagement;

// Check Scene And Change Condition
namespace Gyemong.GameSystem.Map.Quest
{
    public class SceneChecker : MonoBehaviour
    {
        [SerializeField] private string _conditionString;
        [SerializeField] private bool _conditionValue;
        [SerializeField] private string _sceneName;
        [SerializeField] private EventObject _eventObject;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals(_sceneName))
            {
                if (_eventObject.gameObject.activeInHierarchy)
                {
                    _eventObject.Trigger();
                }
                ConditionManager.Instance.Conditions[_conditionString] = _conditionValue;
            }
        }
    }
}
