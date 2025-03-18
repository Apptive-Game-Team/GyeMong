using UnityEngine;

namespace Creature.Attack.Component.Movement
{
    public class FlippingMovement : StaticMovement
    {
        private static bool flipRegister = false;
        public FlippingMovement(Transform transform, Vector3 position, float duration) : base(position, duration)
        {
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y) * (flipRegister? 1 : -1), transform.localScale.z);
        }
    }
}