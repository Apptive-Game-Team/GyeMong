using UnityEngine;

namespace Creature.Attack.Component.Movement
{
    public interface IAttackObjectMovement
    {
        public Vector3 GetPosition(float time);
    }
}