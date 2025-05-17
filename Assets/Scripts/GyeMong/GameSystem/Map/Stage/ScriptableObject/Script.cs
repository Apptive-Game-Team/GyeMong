using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Map.Stage.ScriptableObject
{
    [CreateAssetMenu(fileName = "Script", menuName = "ScriptableObjects/Script", order = 1)]
    public class Script : UnityEngine.ScriptableObject
    {
        [SerializeField] public List<EventSystem.Event.Event> events;
        
        public IEnumerator Execute()
        {
            foreach (EventSystem.Event.Event @event in events)
            {
                yield return @event.Execute();
            }
        }
    }
}