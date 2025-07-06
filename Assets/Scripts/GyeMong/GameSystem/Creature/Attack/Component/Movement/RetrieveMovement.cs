using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Movement
{
    public class RetrieveMovement : IAttackObjectMovement
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _duration;
        
        public RetrieveMovement(Vector3 startPosition, Vector3 targetPosition, float speed)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            float distance = Vector3.Distance(_startPosition, _targetPosition);
            _duration = distance / speed * 2;
        }

        public Vector3? GetPosition(float time)
        {
            if (time > _duration)
            {
                return null;
            }
            else if (time > _duration / 2)
            {
                return Vector3.Lerp(_targetPosition, _startPosition, (time - _duration/2) / _duration);
            }
            return Vector3.Lerp(_startPosition, _targetPosition, time*2 / _duration);
        }
    }
}