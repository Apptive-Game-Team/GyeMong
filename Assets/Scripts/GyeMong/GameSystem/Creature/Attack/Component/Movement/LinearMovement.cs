using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Movement
{
    public class LinearMovement : IAttackObjectMovement
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _duration;
        
        public LinearMovement(Vector3 startPosition, Vector3 targetPosition, float speed)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            float distance = Vector3.Distance(_startPosition, _targetPosition);
            _duration = distance / speed;
        }

        public Vector3? GetPosition(float time)
        {
            if (time > _duration)
            {
                return null;
            }
            return Vector3.Lerp(_startPosition, _targetPosition, time / _duration);
        }
    }
}