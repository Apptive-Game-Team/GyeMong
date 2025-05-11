using Gyemong.EventSystem.Controller.Condition;
using Gyemong.EventSystem.Interface;
using Gyemong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf;
using Gyemong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem;
using UnityEngine;

namespace Gyemong.GameSystem.Map.Boss
{
    public class BossRoomEntrance : MonoBehaviour, IEventTriggerable
    {
        [SerializeField] private Creature.Mob.StateMachineMob.Boss.Boss boss;
        private string bossConditionKey;

        protected void Awake()
        {
            if (bossConditionKey == null)
            {
                bossConditionKey = GetConditionKeyByBoss(boss);
            }
        }

        protected void Start()
        {
            if (ConditionManager.Instance.Conditions.TryGetValue(bossConditionKey, out bool down) && down)
            {
                Destroy(gameObject);
            }
        }

        public void Trigger()
        {
            if (boss is Creature.Mob.StateMachineMob.Boss.Boss bossInstance)
            {
                bossInstance.ChangeState();
            }
        }

        private string GetConditionKeyByBoss(Creature.Mob.StateMachineMob.Boss.Boss boss)
        {
            if (boss is Elf) return "spring_midboss_down";
            if (boss is Golem) return "spring_guardian_down";
            return string.Empty;
        }
    }
}