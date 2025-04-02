using UnityEngine;
using Creature.Boss;
using Creature.Boss.Spring.Elf;
using Creature.Boss.Spring.Golem;
using Creature.Mob.Boss;
using Creature.Mob.Boss.Spring.Elf;
using Creature.Mob.StateMachineMob.Boss;
using Creature.Mob.StateMachineMob.Boss.Spring.Golem;
using System.Sound;

public class BossRoomEntrance : MonoBehaviour, IEventTriggerable
{
    [SerializeField] private Boss boss;
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
        if (boss is Boss bossInstance)
        {
            bossInstance.ChangeState();
        }
    }

    private string GetConditionKeyByBoss(Boss boss)
    {
        if (boss is Elf) return "spring_midboss_down";
        if (boss is Golem) return "spring_guardian_down";
        return string.Empty;
    }
}