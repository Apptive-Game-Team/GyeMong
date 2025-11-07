using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class AnimationEnd : MonoBehaviour
    {
        public void OnAnimationEnd()
        {
            Destroy(gameObject);
        }
    }
}
