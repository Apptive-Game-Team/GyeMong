using System.Collections;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Indicator;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.SoundSystem;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BattleTestDummy : StateMachineMob
{
    
    protected IDetector<PlayerCharacter> _detector;
    [SerializeField] private GameObject basicAttackPrefab;
    [SerializeField] private GameObject daggerPrefab;

    public static Direction GetDirectionToTarget(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? Direction.Left : Direction.Right;
        else
            return dir.y > 0 ? Direction.Up : Direction.Down;
    }
    
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
        StageManager.ClearStage(this);
    }

    private void Start()
    {
        Initialize();
        ChangeState(new DetectingPlayer(){mob = this});
    }

    protected void Initialize()
    {
        maxHp = 9999;
        currentHp = maxHp;

        currentShield = 0;
        damage = 1;
        speed = 1;
        detectionRange = 10;
        MeleeAttackRange = 2;
        RangedAttackRange = 10;

        _detector = SimplePlayerDistanceDetector.Create(this);
    }
    
    public abstract class DummyState : BaseState
    {
        protected BattleTestDummy Dummy => mob as BattleTestDummy;
    }
    
    private IEnumerator MeleeAttack(GameObject prefab, float distance = 0.5f, float duration = 0.5f)
    {
        FaceToPlayer();
        Direction dir = GetDirectionToTarget(DirectionToPlayer);
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
        Direction dir = GetDirectionToTarget(DirectionToPlayer);
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
        Direction direction = GetDirectionToTarget(DirectionToPlayer);
        _animator.Play($"Move_{direction}");
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

    public IEnumerator ChangeAlpha(float targetAlpha, float duration = 0.3f)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Color startColor = sr.color;
        float startAlpha = startColor.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            sr.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            yield return null;
        }
        
        sr.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
    
    private void FaceToPlayer()
    {
        Animator.SetFloat("xDir", DirectionToPlayer.x);
        Animator.SetFloat("yDir", DirectionToPlayer.y);
    }

    public class DummyThrow : DummyState
    {
        public override int GetWeight()
        {
            return 9999;
        }

        public override IEnumerator StateCoroutine()
        {
            yield return Dummy.RangeThrow(Dummy.daggerPrefab, 1, 10f);
            yield return new WaitForSeconds(1f);
            Dummy.ChangeState(new DetectingPlayer() {mob = Dummy});
        }
    }

    public class DetectingPlayer : DummyState
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
                Transform target = Dummy._detector.DetectTarget()?.transform;
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
