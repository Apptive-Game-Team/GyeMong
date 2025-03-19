using System.Collections;
using Creature.Attack;
using Creature.Attack.Component.Movement;
using UnityEngine;

namespace Creature.Minion.ShadowOfHero
{
    public class ShadowOfHero : Creature
    {
        private int _attackCount = 0;
        private const int MAX_ATTACK_COUNT = 3;
        [SerializeField] private GameObject attackPrefab;
        [SerializeField] private GameObject skillPrefab;
        
        private void Start()
        {
            Initialize();
            
            // for debug

            ChangeState();
        }

        private IEnumerator MeleeAttack()
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            AttackObjectController.Create(
                    transform.position + DirectionToPlayer * 0.5f, 
                    DirectionToPlayer, 
                    attackPrefab, 
                    new StaticMovement(
                        transform.position + DirectionToPlayer * 0.5f, 
                        0.3f)
                )
                .StartRoutine();
            yield return new WaitForSeconds(0.2f);
            _animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(0.1f);
        }
        
        private IEnumerator RangeAttack()
        {
            FaceToPlayer();
            _animator.SetTrigger("isAttacking");
            AttackObjectController.Create(
                transform.position + DirectionToPlayer * 0.5f, 
                DirectionToPlayer, 
                attackPrefab, 
                new StaticMovement(
                    transform.position + DirectionToPlayer * 0.5f, 
                    0.3f)
                )
                .StartRoutine();
            AttackObjectController.Create(
                transform.position + DirectionToPlayer * 0.8f, 
                DirectionToPlayer, 
                skillPrefab, 
                new LinearMovement(
                    transform.position, 
                    transform.position + DirectionToPlayer * 10, 
                    10)
                )
                .StartRoutine();
            yield return new WaitForSeconds(0.2f);
            _animator.SetBool("isAttacking", false);
            yield return new WaitForSeconds(0.1f);
        }

        protected void Initialize()
        {
            maxHp = 100;
            currentHp = maxHp;

            currentShield = 0;
            damage = 10;
            speed = 1;
            detectionRange = 10;
            MeleeAttackRange = 2;
            RangedAttackRange = 20;
        }

        private void FaceToPlayer()
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
                while (creature.DistanceToPlayer > creature.MeleeAttackRange)
                {
                     creature.TrackPlayer();
                     ShadowOfHero.FaceToPlayer();
                     yield return null;
                }
                creature.Animator.SetBool("isMove", false);
                yield return null;
                creature.ChangeState();
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
                for (int i = 0; i < 10; i++)
                {
                    yield return ShadowOfHero.RangeAttack();
                }
                
                yield return new WaitForSeconds(5f);
                creature.ChangeState();
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
                ShadowOfHero._attackCount += 1;
                yield return ShadowOfHero.MeleeAttack();
                creature.ChangeState();
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
                creature.Animator.SetBool("isDashing", true);
                yield return creature.RushAttack();
                creature.Animator.SetBool("isDashing", false);
                yield return ShadowOfHero.MeleeAttack();
                creature.ChangeState();
            }
        }

        public abstract class ShadowState : BaseState
        {
            protected ShadowOfHero ShadowOfHero => creature as ShadowOfHero;
        }
    }
}
