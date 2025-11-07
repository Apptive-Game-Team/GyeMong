using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Movement
{
    public class ParabolicMovement : IAttackObjectMovement
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _duration;
        private float _arcHeight;

        public ParabolicMovement(Vector3 start, Vector3 target, float speed, float arcHeight = 1f)
        {
            _startPosition = start;
            _targetPosition = target;
            float distance = Vector3.Distance(start, target);
            _duration = distance / speed;
            _arcHeight = arcHeight;
        }

        public Vector3? GetPosition(float time)
        {
            if (time > _duration)
                return null;

            float t = time / _duration;
            
            Vector3 linearPos = Vector3.Lerp(_startPosition, _targetPosition, t);
            
            float height = Mathf.Sin(t * Mathf.PI) * _arcHeight;
            linearPos.y += height;

            return linearPos;
        }
        
        public Vector3? GetDirection(float time)
        {
            if (time > _duration) return null;

            float tNorm = Mathf.Clamp01(time / _duration);
            Vector3 delta = _targetPosition - _startPosition;
            
            Vector3 v = delta / _duration;
            
            float dy = Mathf.Cos(tNorm * Mathf.PI) * Mathf.PI * _arcHeight / _duration;
            v.y += dy;
            
            float mag = v.magnitude;
            return v / mag;
        }
    }
}
