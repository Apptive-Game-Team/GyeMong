using System;
using System.Collections;
using playerCharacter;
using UnityEngine;

public class Slime : Creature
{
    [SerializeField] private GameObject rangedAttack;
    private IDetector<PlayerCharacter> _detector;
    private IPathFinder _pathFinder;
    private SlimeAnimator _slimeAnimator;
    [SerializeField] private SlimeSprites sprites;
    private Coroutine faceToPlayerCoroutine;
    
    public override void OnAttacked(float damage)
    {
        base.OnAttacked(damage);
        if (currentHp <= 0)
        {
            ChangeState(new SlimeDieState(this));
        }
    }
    
    public IEnumerator FaceToPlayer()
    {
        float scale = transform.localScale.x;
        while (true)
        {
            if (PlayerCharacter.Instance.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-scale, scale, scale);
            }
            else
            {
                transform.localScale = new Vector3(scale, scale, scale);
            }
            yield return null;
        }
    }
    
    private void Start()
    {
        Initialize();
        ChangeState();
        faceToPlayerCoroutine = StartCoroutine(FaceToPlayer());
    }

    private void Initialize()
    {
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
            (creature as Slime)?._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.RANGED_ATTACK);
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
            GameObject arrow =  Instantiate((creature as Slime).rangedAttack, creature.transform.position, Quaternion.identity);
            (creature as Slime).RotateArrowTowardsPlayer(arrow);
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
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
            (creature as Slime)?._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK);
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME * 2);
            PlayerCharacter.Instance.TakeDamage(creature.damage);
            yield return new WaitForSeconds(SlimeAnimator.ANIMATION_DELTA_TIME);
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
            (creature as Slime)?._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MELEE_ATTACK, true);
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
    
    public class SlimeDieState : BaseState
    {
        public SlimeDieState() { }
        public SlimeDieState(Creature creature)
        {
            this.creature = creature;
        }
        public override int GetWeight()
        {
            return 0;
        }

        public override IEnumerator StateCoroutine()
        {
            (creature as Slime)?.StopCoroutine((creature as Slime)?.faceToPlayerCoroutine);
            (creature as Slime)?._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.DIE);
            yield return null;
        }
    }
    
    private void RotateArrowTowardsPlayer(GameObject arrow)
    {
        Vector3 direction = DirectionToPlayer; 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
