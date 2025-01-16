using System;
using System.Collections;
using playerCharacter;
using UnityEngine;

public class Slime : Creature
{
    private IDetector<PlayerCharacter> _detector;
    private IPathFinder _pathFinder;
    private SlimeAnimator _slimeAnimator;
    [SerializeField] private SlimeSprites sprites;
    private void Start()
    {
        Initialize();
        ChangeState();
    }

    private void Initialize()
    {
        maxHp = 50;
        currentHp = maxHp;
        currentShield = 0;
        damage = 10;
        speed = 2;
        detectionRange = 10;
        MeleeAttackRange = 1;
        RangedAttackRange = 5;

        _detector = SimplePlayerDetector.Create(this);
        _pathFinder = new SimplePathFinder();
        _slimeAnimator = SlimeAnimator.Create(gameObject, sprites);
    }

    public class RangedAttackState : BaseState
    {
        public override int GetWeight()
        {
            return creature.DistanceToPlayer > creature.MeleeAttackRange && creature.DistanceToPlayer < creature.RangedAttackRange ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            yield return (creature as Slime)?._slimeAnimator.SyncPlay(SlimeAnimator.AnimationType.RANGED_ATTACK);
            (creature as Slime)?._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
            yield return new WaitForSeconds(1);
            creature.ChangeState();
        }
    }
    
    public class MeleeAttackState : BaseState
    {
        public override int GetWeight()
        {
            return creature.DistanceToPlayer <= creature.MeleeAttackRange ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            yield return (creature as Slime)?._slimeAnimator.SyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK);
            (creature as Slime)?._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
            yield return new WaitForSeconds(1);
            creature.ChangeState();
        }
    }
    
    public class SlimeMoveState : BaseState
    {
        public override int GetWeight()
        {
            return creature.DistanceToPlayer > creature.MeleeAttackRange ? 5 : 0;
        }

        public override IEnumerator StateCoroutine()
        {
            (creature as Slime)?._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.IDLE, true);
            float duration = 2f;
            float timer = 0f;
            
            while (duration > timer && creature.DistanceToPlayer > creature.MeleeAttackRange)
            {
                timer += Time.deltaTime;
                yield return null;
                creature.TrackPath((creature as Slime)?._pathFinder.FindPath(creature.transform.position, PlayerCharacter.Instance.transform.position));
            }
            (creature as Slime)?._slimeAnimator.Stop();
            creature.ChangeState();
        }
    }
}
