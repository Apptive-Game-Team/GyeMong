using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Movement
{
    public class ChildMovement : IAttackObjectMovement
    {
        private float _duration;
        Vector3 _offset;
        Transform _parentTransform;
        public ChildMovement(Transform parentTransform, Vector3 offset, float duration)
        {
            _duration = duration;
            _offset = offset;
            _parentTransform = parentTransform;
        }
        
        public Vector3? GetPosition(float time)
        {
            if (time > _duration)
            {
                return null;
            }

            return _parentTransform.position + _offset;
        }
        
        public Vector3? GetDirection(float time)
        {
            return null;
        }
    }
}