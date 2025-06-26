using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Movement
{
    public class CurveMovement : IAttackObjectMovement
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _duration;
        private float _curveAmount;
        
        public CurveMovement(Vector3 startPosition, Vector3 targetPosition, float speed, float curveAmount)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _curveAmount = curveAmount;
            float distance = Vector3.Distance(_startPosition, _targetPosition);
            _duration = distance / speed;
        }

        public Vector3? GetPosition(float time)
        {
            if (time > _duration)
            {
                return null;
            }
            float t = time / _duration;
            
            Vector3 linearPos = Vector3.Lerp(_startPosition, _targetPosition, t);

            Vector3 direction = (_targetPosition - _startPosition).normalized;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0); 
            
            Vector3 curveOffset = perpendicular * (Mathf.Sin(t * Mathf.PI) * _curveAmount);

            return linearPos + curveOffset;
        }
    }
}