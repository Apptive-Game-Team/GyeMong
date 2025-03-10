using UnityEngine;

namespace System.Game
{
    public class DynamicGameObjectCreator : MonoBehaviour
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] [Tooltip("If you want to Summon This GameObject, conditionKey1 == true")] private string _conditionKey1 = "";
        [SerializeField] [Tooltip("If you want to Summon This GameObject, conditionKey2 == false")] private string _conditionKey2 = "";

        private void Start()
        {
            bool condition1 = true;
            bool condition2 = false;

            if (!string.IsNullOrEmpty(_conditionKey1))
            {
                if (ConditionManager.Instance.Conditions.TryGetValue(_conditionKey1, out bool value1))
                {
                    condition1 = value1;
                }
            }
        
            if (!string.IsNullOrEmpty(_conditionKey2))
            {
                if (ConditionManager.Instance.Conditions.TryGetValue(_conditionKey2, out bool value2))
                {
                    condition2 = value2;
                }
            }

            if (condition1 && !condition2)
            {
                Instantiate(_prefab);
            }
        }
    }

}
