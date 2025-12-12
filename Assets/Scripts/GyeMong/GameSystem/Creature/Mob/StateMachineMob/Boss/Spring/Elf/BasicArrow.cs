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
            Destroy(gameObject);
            yield return null;
        }
    }
}