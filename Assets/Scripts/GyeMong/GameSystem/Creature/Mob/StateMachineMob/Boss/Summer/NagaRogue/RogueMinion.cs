using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.ShadowOfHero;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;

public class RogueMinion : StateMachineMob
{
    
    protected IDetector<PlayerCharacter> _detector;
    [SerializeField] private GameObject throwPrefab;
    
    public override void OnAttacked(float damage)
    {
        currentHp -= damage;
        
        if (currentHp <= 0)
        {
            OnDead();
        }
        else
        {
            StartCoroutine(Blink());
        }
    }

    protected override void OnDead()
    {
        Destroy(this.gameObject);
    }

    private void Start()
    {
        Initialize();
        ChangeState(new DetectingPlayer() {mob = this});
    }

    protected void Initialize()
    {
        maxHp = 30;
        currentHp = maxHp;

        currentShield = 0;
        damage = 10;
        speed = 1;
        detectionRange = 20;
        MeleeAttackRange = 2;
        RangedAttackRange = 20;

        _detector = SimplePlayerDistanceDetector.Create(this);
    }
    
    public abstract class RogueMinionState : BaseState
    {
        protected RogueMinion RogueMinion => mob as RogueMinion;
    }
    
    private IEnumerator MeleeAttack(GameObject prefab, float distance = 0.5f, float duration = 0.5f)
    {
        FaceToPlayer();
        _animator.SetTrigger("isAttacking");
        AttackObjectController.Create(
                transform.position + DirectionToPlayer * distance, 
                DirectionToPlayer, 
                prefab, 
                new StaticMovement(
                    transform.position + DirectionToPlayer * distance, 
                    duration)
            )
            .StartRoutine();
        yield return new WaitForSeconds(0.2f);
    }
    private IEnumerator RangeThrow(GameObject prefab, float distance = 0.5f, float throwSpeed = 10f)
    {
        FaceToPlayer();
        _animator.SetTrigger("isAttacking");
        AttackObjectController.Create(
                transform.position + DirectionToPlayer * distance, 
                DirectionToPlayer, 
                prefab, 
                new LinearMovement(
                    transform.position + DirectionToPlayer * distance, 
                    SceneContext.Character.transform.position,
                    throwSpeed)
            )
            .StartRoutine();
        yield return new WaitForSeconds(0.2f);
    }
    
    public IEnumerator Move(Vector3 dir, float distance = 2f,float duration = 0.5f)
    {
        float elapsed = 0f;
        Vector3 destination = transform.position + dir * distance;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(transform.position, destination, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = destination;
    }
    
    private void FaceToPlayer()
    {
        Animator.SetFloat("xDir", DirectionToPlayer.x);
        Animator.SetFloat("yDir", DirectionToPlayer.y);
    }
    
    public class ThrowAttack : RogueMinionState
    {
        public override int GetWeight()
        {
            return 50;
        }

        public override IEnumerator StateCoroutine()
        {
            yield return RogueMinion.RangeThrow(RogueMinion.throwPrefab);
            yield return new WaitForSeconds(1f);
            RogueMinion.ChangeState(new DetectingPlayer() {mob = RogueMinion});
        }
    }
    
    public class DetectingPlayer : RogueMinionState
    {
        public override int GetWeight()
        {
            return 0;
        }

        public override IEnumerator StateCoroutine()
        {
            Debug.Log("Detecting Player");
            mob.Animator.SetBool("isMove", false);
            while (true)
            {
                Transform target = RogueMinion._detector.DetectTarget()?.transform;
                if (target != null)
                {
                    mob.ChangeState();
                    yield break;
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
    
}
