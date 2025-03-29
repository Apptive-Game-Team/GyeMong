using UnityEngine;

namespace Creature.Attack.Component.Movement
{
    public class StaticMovement : IAttackObjectMovement
    {
        private float _duration;
        Vector3 _position;
        public StaticMovement(Vector3 position, float duration)
        {
            _duration = duration;
            _position = position;
        }
        
        public Vector3? GetPosition(float time)
        {
            if (time > _duration)
            {
                return null;
            }

            return _position;
        }
    }
}