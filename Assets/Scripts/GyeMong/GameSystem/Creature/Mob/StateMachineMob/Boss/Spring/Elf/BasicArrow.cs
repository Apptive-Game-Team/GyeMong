using System;
using System.Collections;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class BasicArrow : ArrowBase
    {
        protected override IEnumerator OnReachEnd()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }
    }
}