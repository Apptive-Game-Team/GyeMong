
using System.Collections;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaRogueScript
{
    public class NagaRogueAction
    {
        private readonly NagaRogue _o;

        public NagaRogueAction(NagaRogue owner) { _o = owner; }

        public static Direction GetDirectionToTarget(Vector2 dir)
        {
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                return dir.x > 0 ? Direction.Left : Direction.Right;
            else
                return dir.y > 0 ? Direction.Up : Direction.Down;
        }

    }
}
