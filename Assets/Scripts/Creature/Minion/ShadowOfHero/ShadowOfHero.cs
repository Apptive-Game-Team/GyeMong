using System;
using System.Collections;
using playerCharacter;

namespace Creature.Minion.ShadowOfHero
{
    public class ShadowOfHero : Creature
    {
        private int _attackCount = 0;
        private const int MAX_ATTACK_COUNT = 3;
        
        

        private void Start()
        {
            Initialize();
        }

        private IEnumerator MeleeAttack()
        {
            return null;
        }

        protected void Initialize()
        {
            maxHp = 100;
            currentHp = maxHp;

            currentShield = 0;
            damage = 10;
            speed = 5;
            detectionRange = 10;
            MeleeAttackRange = 5;
            RangedAttackRange = 20;
        }

        public void FaceToPlayer()
        {
            Animator.SetFloat("xDir", DirectionToPlayer.x);
            Animator.SetFloat("yDir", DirectionToPlayer.y);
        }
        
        public class WalkState : ShadowState
        {
            public override int GetWeight()
            {
                if (creature.DistanceToPlayer > creature.MeleeAttackRange)
                {
                    return 5;
                }
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                creature.Animator.SetBool("isMove", true);
                while (creature.DistanceToPlayer < creature.MeleeAttackRange)
                {
                     creature.TrackPlayer();
                     ShadowOfHero.FaceToPlayer();
                     return null;
                }
                creature.Animator.SetBool("isMove", false);
                return null;
            }
        }
        
        public class ComboAttackState : ShadowState
        {
            public override int GetWeight()
            {
                if (ShadowOfHero._attackCount >= MAX_ATTACK_COUNT && creature.DistanceToPlayer < creature.MeleeAttackRange)
                {
                    ShadowOfHero._attackCount = 0;
                    return 100;
                }
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                throw new System.NotImplementedException();
            }
        }
       
        public class AttackState : ShadowState
        {
            public override int GetWeight()
            {
                if (creature.DistanceToPlayer < creature.MeleeAttackRange)
                    return 5;
                return 0;
            }
            
            public override IEnumerator StateCoroutine()
            {
                throw new System.NotImplementedException();
            }
        }
        
        public class DashAttackState : ShadowState
        {
            public override int GetWeight()
            {
                if (creature.DistanceToPlayer > creature.MeleeAttackRange && creature.DistanceToPlayer < creature.RangedAttackRange)
                {
                    return 5;
                }
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                ShadowOfHero._attackCount += 1;
                throw new System.NotImplementedException();
            }
        }

        public abstract class ShadowState : BaseState
        {
            protected ShadowOfHero ShadowOfHero => creature as ShadowOfHero;
        }
    }
}
