using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime;
using GyeMong.GameSystem.Map.Stage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Wanderer
{
    public class WandererManager : MonoBehaviour
    {
        public static WandererManager Instance;

        [SerializeField] private List<MultiChatMessageData> afterScript;
        private void Awake()
        {
            if (Instance != null) Destroy(this);
            else Instance = this;
        }
        public void StartWandererDeath()
        {
            StageManager.ClearStage(this);
        }
    }
}
