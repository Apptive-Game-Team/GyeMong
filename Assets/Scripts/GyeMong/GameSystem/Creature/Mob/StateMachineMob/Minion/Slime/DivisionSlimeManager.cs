using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime
{
    public class DivisionSlimeManager : MonoBehaviour
    {
        public static DivisionSlimeManager Instance;
        private HashSet<DivisionSlime> _activeSlimes = new HashSet<DivisionSlime>();
        [SerializeField] private List<MultiChatMessageData> afterScript;

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            else Instance = this;
        }
        
        public void RegisterSlime(DivisionSlime slime)
        {
            _activeSlimes.Add(slime);
        }
        
        public void UnregisterSlime(DivisionSlime slime)
        {
            _activeSlimes.Remove(slime);
            StartCoroutine(CheckAllSlimesDead());
        }
        
        private IEnumerator CheckAllSlimesDead()
        {
            if (_activeSlimes.Count == 0)
            {
                if (afterScript != null)
                {
                    foreach (var script in afterScript)
                    {
                        yield return script.Play();
                    }
                }
                StageManager.ClearStage(this);
            }
        }
    }
}

