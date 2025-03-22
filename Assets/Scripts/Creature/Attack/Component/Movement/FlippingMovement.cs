using UnityEngine;

namespace Creature.Attack.Component.Movement
{
    public class FlippingMovement : StaticMovement
    {
        private static bool _wasFliped  = false;
        public FlippingMovement(Transform transform, Vector3 position, float duration) : base(position, duration)
        {
            _wasFliped = !_wasFliped;
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y) * (_wasFliped? 1 : -1), transform.localScale.z);
        }
    }
}