using UnityEngine;

namespace Creature.Mob.StateMachineMob.Minion.Slime.Components
{
    [CreateAssetMenu(fileName = "SlimeSprites", menuName = "ScriptableObject/SlimeSprites")]
    public class SlimeSprites : ScriptableObject
    {
        public Sprite[] idleSprites;
        public Sprite[] rangedAttackSprites;
        public Sprite[] meleeAttackSprites;
        public Sprite[] dieSprites;
    
        public Sprite[] GetSprite(SlimeAnimator.AnimationType type)
        {
            switch (type)
            {
                case SlimeAnimator.AnimationType.Idle:
                    return idleSprites;
                case SlimeAnimator.AnimationType.MeleeAttack:
                    return meleeAttackSprites;
                case SlimeAnimator.AnimationType.RangedAttack:
                    return rangedAttackSprites;
                case SlimeAnimator.AnimationType.Die:
                    return dieSprites;
            }
            return null;
        }
    }
}