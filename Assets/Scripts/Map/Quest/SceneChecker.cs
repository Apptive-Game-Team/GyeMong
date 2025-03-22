using UnityEngine;
using UnityEngine.SceneManagement;

// Check Scene And Change Condition
namespace Map.Quest
{
    public class SceneChecker : MonoBehaviour
    {
        [SerializeField] private string _conditionString;
        [SerializeField] private bool _conditionValue;
        [SerializeField] private string _sceneName;

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
                ConditionManager.Instance.Conditions[_conditionString] = _conditionValue;
            }
        }
    }
}
