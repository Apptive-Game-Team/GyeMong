using System.Collections.Generic;
using GyeMong.EventSystem;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime
{
    public class DivisionSlimeManager : MonoBehaviour
    {
        public static DivisionSlimeManager Instance;
        private HashSet<DivisionSlime> _activeSlimes = new HashSet<DivisionSlime>();
        private EventObject _eventObject;

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            else Instance = this;
        }

        private void Start()
        {
            _eventObject = GetComponent<EventObject>();
        }
        
        public void RegisterSlime(DivisionSlime slime)
        {
            _activeSlimes.Add(slime);
        }
        
        public void UnregisterSlime(DivisionSlime slime)
        {
            _activeSlimes.Remove(slime);
            CheckAllSlimesDead();
        }
        
        private void CheckAllSlimesDead()
        {
            if (_activeSlimes.Count == 0)
            {
                StageManager.ClearStage(this);
            }
        }
    }
}

