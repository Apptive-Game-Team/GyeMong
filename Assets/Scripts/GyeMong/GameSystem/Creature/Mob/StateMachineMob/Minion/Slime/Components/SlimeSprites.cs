using System;
using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components
{
    [CreateAssetMenu(fileName = "SlimeSprites", menuName = "ScriptableObject/SlimeSprites")]
    public class SlimeSprites : ScriptableObject
    {
        [Serializable]
        public class Sprites
        {
            public Sprite[] sprites;
        }
        
        public Sprites[] idleSprites;
        public Sprites[] rangedAttackSprites;
        public Sprites[] meleeAttackSprites;
        public Sprites[] dashAttackSprites;
        public Sprites[] dieSprites;
    
        public Sprite[] GetSprite(SlimeAnimator.AnimationType type, int dir = 0)
        {
            switch (type)
            {
                case SlimeAnimator.AnimationType.Idle:
                    return idleSprites[dir].sprites;
                case SlimeAnimator.AnimationType.MeleeAttack:
                    return meleeAttackSprites[dir].sprites;
                case SlimeAnimator.AnimationType.RangedAttack:
                    return rangedAttackSprites[dir].sprites;
                case SlimeAnimator.AnimationType.DashAttack:
                    return dashAttackSprites[dir].sprites;
                case SlimeAnimator.AnimationType.Die:
                    return dieSprites[dir].sprites;
            }
            return null;
        }
    }
}