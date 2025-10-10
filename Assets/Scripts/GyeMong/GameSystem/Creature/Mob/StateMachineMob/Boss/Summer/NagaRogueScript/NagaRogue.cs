using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Indicator;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.SoundSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaRogueScript
{
    public enum Direction
    {
        Up, Down, Right, Left
    }

    public class NagaRogue : StateMachineMob
    {
    
        protected IDetector<PlayerCharacter> _detector;
        [SerializeField] private GameObject slashAttackPrefab;
        [SerializeField] private GameObject thrustAttackPrefab;
        [SerializeField] private GameObject daggerPrefab;
        [SerializeField] private GameObject shurikenPrefab;
        [SerializeField] private GameObject handEffectPrefab;
        [SerializeField] private ParticleSystem sandstormPrefab;
        [SerializeField] private MultiChatMessageData phaseChangeChat;
        [SerializeField] private Transform centerPoint;

        private List<GameObject> daggerList = new();
        private int daggerCap = 6;
        public const float DaggerRange = 10f;
        private float _phaseChangeHealthPercent = 0.66f;
        private float _lastDitchHealthPercent = 0.33f;
        public bool canPhaseChange = true;
        public bool canLastDitch = true;
        
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
            //test
            ChangeState();
        }

        protected void Initialize()
        {
            maxHp = 100;
            currentHp = maxHp;

            currentShield = 0;
            damage = 10;
            speed = 1;
            detectionRange = 50;
            MeleeAttackRange = 3;
            RangedAttackRange = 10;

            _detector = SimplePlayerDistanceDetector.Create(this);
        }
    
        public abstract class NagaRogueState : BaseState
        {
            protected NagaRogue NagaRogue => mob as NagaRogue;
            protected float Cooldown = 0f;
        }

        private IEnumerator SetIdleAnim(float delay)
        {
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Idle_{dir}", 0, 0f);
            yield return new WaitForSeconds(delay);
        }
        private IEnumerator MeleeAttack(GameObject prefab, float duration = 0.3f)
        {
            const float startPosDist = 1f;
            FaceToPlayer();
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Stab_{dir}", 0, 0f);
            Vector3 targetPos = transform.position + DirectionToPlayer * startPosDist;
            Vector3 targetDir = DirectionToPlayer; 
            AttackObjectController.Create(targetPos, targetDir, prefab, 
                    new StaticMovement(targetPos, duration)
                ).StartRoutine(); 
            yield return new WaitForSeconds(0.2f);
        }
        private IEnumerator ThrustAttack(GameObject prefab, float duration = 0.3f)
        {
            const float startPosDist = 2f;
            FaceToPlayer();
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Throw_{dir}", 0, 0f);
            Vector3 targetPos = transform.position + DirectionToPlayer * startPosDist;
            Vector3 targetDir = DirectionToPlayer;
            StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator(prefab, targetPos, Quaternion.FromToRotation(Vector3.right, targetDir), duration));
            yield return new WaitForSeconds(duration);
            AttackObjectController.Create(
                targetPos, 
                targetDir, 
                prefab, 
                new StaticMovement(
                    targetPos, 
                    duration)
            ).StartRoutine();
            Vector3 spawnPos = transform.position + DirectionToPlayer * 0.5f;
            GameObject go = Instantiate(handEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(go,0.5f);
            yield return null;
        }

        private IEnumerator RetrieveObject(GameObject prefab, float retrieveSpeed = 40f)
        {
            FaceToPlayer();
            Vector3 dirToPlayer = SceneContext.Character.transform.position - prefab.transform.position;
            Direction direction = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Throw_{direction}", -1, 0f);
            Sound.Play("ENEMY_NagaRogue_Retrieve");
            AttackObjectController ac = AttackObjectController.Create(
                SceneContext.Character.transform.position,
                dirToPlayer,
                prefab,
                new LinearMovement(
                    prefab.transform.position,
                    transform.position,
                    retrieveSpeed)
            );
            ac.StartRoutine();
            ac.gameObject.GetComponent<ReturnDagger>().Initiate(transform);
            Vector3 spawnPos = transform.position + DirectionToPlayer * 0.5f;
            GameObject go = Instantiate(handEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(go,0.5f);
            yield return null;
        }
    
        private IEnumerator CurveThrow(GameObject prefab, float distance = 0.5f, float throwSpeed = 8f, float curveAmount = 0.2f)
        {
            Sound.Play("ENEMY_NagaRogue_Throw");
            FaceToPlayer();
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Vector3 spawnPos = transform.position + DirectionToPlayer * distance;
            Animator.Play($"Throw_{dir}",0,0f);
            AttackObjectController.Create(
                    spawnPos, 
                    DirectionToPlayer, 
                    prefab, 
                    new BoomerangMovement(
                        spawnPos, 
                        SceneContext.Character.transform.position,
                        throwSpeed,
                        curveAmount)
                )
                .StartRoutine();
            GameObject go = Instantiate(handEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(go,0.5f);
            yield return new WaitForSeconds(0.5f);
        }
        
        private IEnumerator RangeThrow(
            GameObject prefab,
            float daggerRange = 10f,
            float throwSpeed  = 20f,
            float aimErrorDeg = 0f        // ← 추가: 오차 각(도)
        )
        {
            FaceToPlayer();
            Direction dirEnum = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Sound.Play("ENEMY_NagaRogue_Throw");
            Animator.Play($"Throw_{dirEnum}", 0, 0f);

            // 기준 방향 + 랜덤 오차
            Vector3 forward = DirectionToPlayer.normalized;
            if (aimErrorDeg > 0f)
            {
                float err = Random.Range(-aimErrorDeg, aimErrorDeg);
                forward = Quaternion.Euler(0, 0, err) * forward;
            }

            Vector3 spawnPos = transform.position + forward * 0.6f;
            Vector3 targetPos = spawnPos + forward * daggerRange;

            var ac = AttackObjectController.Create(
                spawnPos, forward, prefab,
                new LinearMovement(spawnPos, targetPos, throwSpeed)
            );
            ac.gameObject.GetComponent<ReturnDagger>().Initiate(transform);
            ac.StartRoutineWithCallOnEnd(() => AddDagger(ac.gameObject));
            GameObject go = Instantiate(handEffectPrefab, spawnPos, Quaternion.identity);
            Destroy(go,0.5f);

            yield return new WaitForSeconds(0.2f);
        }
        
        private IEnumerator DaggerFanThrow(GameObject prefab, int daggerCount = 3, 
            float spreadAngle = 120f,
            float throwSpeed = 20f, float daggerRange = 15f)
        {
            FaceToPlayer();
            Direction direction = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Throw_{direction}", -1, 0f);

            Vector3 origin = transform.position;
            Vector3 toPlayer = (SceneContext.Character.transform.position - origin).normalized;
            float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

            float startAngle = baseAngle - (spreadAngle / 2f);
            float angleStep = spreadAngle / (daggerCount - 1);
            
            for (int i = 0; i < daggerCount; i++)
            {
                float angle = startAngle + angleStep * i;

                float rad = angle * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;

                Vector3 spawnPos = origin + dir * 0.6f;
                Vector3 targetPos = spawnPos + dir * daggerRange;

                var ac = AttackObjectController.Create(
                    spawnPos, dir, prefab,
                    new LinearMovement(spawnPos, targetPos, throwSpeed)
                );
                ac.gameObject.GetComponent<ReturnDagger>().Initiate(transform);
                ac.StartRoutineWithCallOnEnd(() => AddDagger(ac.gameObject));
                yield return new WaitForSeconds(0.015f);
            }
        }
//---------------------------------여러 방향 선택하는 알고리즘들-----------------------------

        private void AddDagger(GameObject obj)
        {
            if (daggerList.Count > daggerCap)
            {
                Destroy(daggerList[0]);
                daggerList.RemoveAt(0);
            }
            daggerList.Add(obj);
        }

private Vector3 Perp(Vector3 v) => new Vector3(-v.y, v.x, 0).normalized;

private bool InViewport(Vector3 world, float margin = 0.06f) {
    var cam = Camera.main;
    if (!cam) return true;
    var v = cam.WorldToViewportPoint(world);
    return v.z > 0 && v.x > margin && v.x < 1 - margin && v.y > margin && v.y < 1 - margin;
}

private Vector3 GetRandomDaggerPos()
{
    int rnd = Random.Range(0, daggerList.Count);
    return daggerList[rnd].transform.position;
}

// 해당 지점이 'Obstacle'과 겹치지 않는지 검사(보스 콜라이더 크기 기준)
private bool IsPointFree(Vector3 pos, float skin = 0.04f)
{
    var col = GetComponent<Collider2D>();
    int obstacleMask = LayerMask.GetMask("Obstacle");

    if (!col) // 콜라이더가 없다면 포인트만 검사(간단 폴백)
        return Physics2D.OverlapPoint(pos, obstacleMask) == null;

    Vector2 size = (Vector2)col.bounds.size - new Vector2(skin, skin);
    size.x = Mathf.Max(0.01f, size.x);
    size.y = Mathf.Max(0.01f, size.y);
    return Physics2D.OverlapBox(pos, size, 0f, obstacleMask) == null;
}

// P(플레이어)에서 d만큼 떨어진 연장선상의 '유효한' 지점을 찾는다.
// 못 찾으면 약간 앞/뒤, 좌/우로 샘플링해서 가장 가까운 유효 지점을 반환.
        

private Vector3 FindRetrievePoint(float d,
    int alongSamples = 8, float alongStep = 0.35f,
    int lateralSamples = 6, float lateralStep = 0.3f)
{
    Vector3 p = SceneContext.Character.transform.position;     // 플레이어
    Vector3 c = GetRandomDaggerPos();                          
    Vector3 dir = (p - c);
    dir.Normalize();

    
    // 1) 기본 지점: P에서 dir로 d만큼 떨어진 곳
    Vector3 desired = p + dir * d;
    if (IsPointFree(desired)) return desired;

    // 2) 선을 따라 앞/뒤로 조금씩 움직이며 검색
    for (int i = 1; i <= alongSamples; i++)
    {
        float s = i * alongStep;
        Vector3 fwd = p + dir * (d + s);
        if (IsPointFree(fwd)) return fwd;

        Vector3 back = p + dir * (d - s);
        if (IsPointFree(back)) return back;
    }

    // 3) 수직 방향(좌/우)으로 살짝 비켜서 검색
    Vector3 perp = new Vector3(-dir.y, dir.x, 0f);
    for (int i = 1; i <= lateralSamples; i++)
    {
        float o = i * lateralStep;
        Vector3 left  = desired + perp * o;
        if (IsPointFree(left)) return left;

        Vector3 right = desired - perp * o;
        if (IsPointFree(right)) return right;
    }

    // 4) 끝까지 못 찾으면 기본 지점 반환(장애물과 겹칠 수도 있음)
    return desired;
}



// 장애물 회피용: 원하는 방향(desired) 근처에서 여러 후보를 테스트하고 최고 점수 방향 선택
private Vector3 ChooseDashDirection(Vector3 desired, float distance, int samples = 9, float maxAngle = 75f)
{
    Collider2D col = GetComponent<Collider2D>();
    Vector3 bestDir = desired.normalized;
    float bestScore = -999f;

    for (int i = 0; i < samples; i++)
    {
        float t = (samples == 1) ? 0f : i / (samples - 1f);
        float angle = Mathf.Lerp(-maxAngle, maxAngle, t);
        Vector3 cand = Quaternion.Euler(0,0,angle) * desired.normalized;

        var hit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, cand, distance, LayerMask.GetMask("Obstacle"));
        float clearance = (hit.collider ? hit.distance : distance);
        float align = Vector3.Dot(desired.normalized, cand);
        float score = clearance + align * 0.5f; // 가중치: 직진 성향 0.5

        if (score > bestScore)
        {
            bestScore = score;
            bestDir = cand;
        }
    }
    return bestDir;
}

public IEnumerator Move(Vector3 desiredDir, float distance = 2f, float duration = 0.5f)
{
    // 1) 후보 중 최적 방향 선택
    Vector3 dir = ChooseDashDirection(desiredDir, distance);

    // 2) 히트 시 살짝 ‘슬라이드’: 표면 법선에 직교 방향으로 보정
    Collider2D col = GetComponent<Collider2D>();
    var hit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, dir, distance, LayerMask.GetMask("Obstacle","Player"));

    float moveDist = hit.collider ? Mathf.Max(0f, hit.distance - 0.02f) : distance;
    Vector3 endPos = transform.position + dir * moveDist;

    if (hit.collider && moveDist < distance * 0.6f)
    {
        // 벽에 거의 붙었으면 접선 방향으로 짧게 미끄러지며 틈새 찾기
        Vector2 tangent = new Vector2(-hit.normal.y, hit.normal.x).normalized;
        var sideHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, tangent, distance * 0.5f, LayerMask.GetMask("Obstacle"));
        float sideDist = sideHit.collider ? sideHit.distance : distance * 0.5f;
        endPos += (Vector3)tangent * sideDist * 0.8f;
    }

    Sound.Play("ENEMY_NagaRogue_Step");
    Direction animDir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
    Animator.Play($"Backstep_{animDir}",0,0f);
    yield return transform.DOMove(endPos, duration).SetEase(Ease.InOutCubic).WaitForCompletion();
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
        // NagaRogue 내부에 추가

// 플레이어(center)에서 radius 떨어진 원 위에서,
// 보스 콜라이더가 'Obstacle'과 겹치지 않는 지점을 샘플링으로 찾는다.
// 못 찾으면 반지름을 조금씩 줄이며 재탐색한다.
        private bool GetFreePosInCircle(
            Vector3 center, float radius, out Vector3 found,
            int angleSamples = 8,              // 각도 샘플 수
            int radialSteps = 2, float radialStep = 0.5f, // 반지름 줄이며 재시도
            float skin = 0.04f                  // 콜라이더 여유(겹침 방지)
        ){
            found = transform.position;

            // 보스 콜라이더 크기(월드)로 OverlapBox 검사
            var col = GetComponent<Collider2D>();
            if (!col) { found = center + (transform.position - center).normalized * radius; return true; }

            Vector2 size = (Vector2)col.bounds.size - Vector2.one * skin;
            size = new Vector2(Mathf.Max(0.01f, size.x), Mathf.Max(0.01f, size.y));
            int obstacleMask = LayerMask.GetMask("Obstacle");

            // 샘플링 시작각: 플레이어 -> 보스 방위각 (뒤로 돌기 쉬움)
            float baseAng = Mathf.Atan2((transform.position - center).y, (transform.position - center).x) + math.PI;
            float stepAng = Mathf.PI * 2f / Mathf.Max(1, angleSamples);

            // 가까운 각도부터: 0, +1, -1, +2, -2...
            System.Func<int, int> zig = k => (k % 2 == 0) ? (k / 2) : (-(k / 2 + 1));

            for (int r = 0; r <= radialSteps; r++)
            {
                float rad = Mathf.Max(0.25f, radius - r * radialStep);
                for (int k = 0; k < angleSamples; k++)
                {
                    int off = zig(k);
                    float ang = baseAng + off * stepAng;
                    Vector3 cand = center + new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f) * rad;

                    // 최종 위치에서 Obstacle과 겹치지 않으면 성공
                    var hit = Physics2D.OverlapBox(cand, size, 0f, obstacleMask);
                    if (hit == null)
                    {
                        found = cand;
                        return true;
                    }
                }
            }

            return false; // 못 찾음 (아래에서 대체 처리)
        }

        public IEnumerator Teleport(Vector3 targetPos, float distance = 2f)
        {
            const float PostDelay = 0.1f;
            FaceToPlayer();
            Direction direction = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            _animator.Play($"BackStep_{direction}",-1,0f);
            Sound.Play("ENEMY_NagaRogue_Ambush");
            yield return ChangeAlpha(0f);
            transform.position = targetPos;
            yield return new WaitForSeconds(0.2f);
            FaceToPlayer();
            yield return ChangeAlpha(1f);
            yield return new WaitForSeconds(PostDelay);
        }
    
        private void FaceToPlayer()
        {
            Animator.SetFloat("xDir", DirectionToPlayer.x);
            Animator.SetFloat("yDir", DirectionToPlayer.y);
        }

        //states============================================================================추후 분리
        public class DashSlash : NagaRogueState
        {
            public const float DashRange = 6f;
            private const float MeleeDelay = 0.3f;
            private const float PostDelay = 1f;
            private const float inCooldown = 4f;
            public override int GetWeight()
            {
                return (Time.time >= Cooldown) ? 500 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.Move(NagaRogue.DirectionToPlayer, DashRange);
                yield return new WaitForSeconds(MeleeDelay);
                for (int i = 0; i < 2; i++)
                {
                    yield return NagaRogue.MeleeAttack(NagaRogue.slashAttackPrefab);  
                }

                Cooldown = Time.time + inCooldown;
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }
        public class SideThrust : NagaRogueState
        {
            public const float DashRange = 3f;
            private const float MeleeDelay = 0.3f;
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.MeleeAttackRange) ? 500 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.Move(NagaRogue.DirectionToPlayer, DashRange);
                yield return new WaitForSeconds(MeleeDelay);
                yield return NagaRogue.ThrustAttack(NagaRogue.thrustAttackPrefab);  

                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }
        public class AmbushThrust : NagaRogueState
        {
            public const float ThrustRange = 2f;
            private const float MeleeDelay = 0.2f;
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.MeleeAttackRange) ? 500 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                NagaRogue.GetFreePosInCircle(SceneContext.Character.transform.position, ThrustRange, out var pos);
                yield return NagaRogue.Teleport(pos, ThrustRange);
                yield return new WaitForSeconds(MeleeDelay);
                yield return NagaRogue.ThrustAttack(NagaRogue.thrustAttackPrefab);  

                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }

        public class RetreatThrow : NagaRogueState
        {
            private const float ThrowDelay = 0.2f;
            private const float PostDelay = 1f;
            private const float ThrowSpeed = 15f;
            private const float AimErrorDeg = 15f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.MeleeAttackRange) ? 500 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                NagaRogue.GetFreePosInCircle(SceneContext.Character.transform.position, mob.MeleeAttackRange, out var pos);
                yield return NagaRogue.Teleport(pos, mob.MeleeAttackRange);
                for (int i = 0; i < 2; i++)
                {
                    yield return NagaRogue.RangeThrow(NagaRogue.daggerPrefab, DaggerRange, ThrowSpeed, AimErrorDeg);
                    yield return new WaitForSeconds(ThrowDelay);
                }
                
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }
        
        public class SequentialDaggerThrow : NagaRogueState
        {
            private const float ThrowDelay = 0.2f;
            private const float PostDelay = 1f;
            private const float ThrowSpeed = 15f;
            private const float AimErrorDeg = 15f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer > mob.MeleeAttackRange) ? 500 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                int iter = Random.Range(1, 4);
                for (int i = 0; i < iter; i++)
                {
                    yield return NagaRogue.RangeThrow(NagaRogue.daggerPrefab, DaggerRange, ThrowSpeed, AimErrorDeg);
                    yield return new WaitForSeconds(ThrowDelay);
                }
                
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }
        
public class DaggerRetrieve : NagaRogueState
{
    private const float PostDelay = 1f;
    private const float MaxRetrieveAngle = 30f; // deg

    public override int GetWeight()
    {
        if (NagaRogue.daggerList.Count >= 5)
        {
            return 50000;
        }
        return (CountDaggersWithinAngle(MaxRetrieveAngle) >= 1 && NagaRogue.daggerList.Count >= 1 ? 500 : 0);
    }

    public override IEnumerator StateCoroutine()
    {
        var boss = NagaRogue;

        Vector2 bossPos   = boss.transform.position;
        Vector2 playerPos = SceneContext.Character.transform.position;
        Vector2 forward   = (playerPos - bossPos).normalized;

        float cosLimit = Mathf.Cos(MaxRetrieveAngle * Mathf.Deg2Rad);
        
        List<GameObject> selected = new();
        foreach (var d in boss.daggerList)
        {
            if (!d || !d.activeInHierarchy) continue;
            Vector2 toDagger = (Vector2)d.transform.position - bossPos;
            if (toDagger.sqrMagnitude < 0.000001f) continue;

            Vector2 dir = toDagger.normalized;
            float dot = Vector2.Dot(dir, forward);
            if (dot > 0f && dot >= cosLimit)
                selected.Add(d);
        }

        if (selected.Count > 0)
        {
            selected.Sort((a, b) =>
            {
                float da = ((Vector2)a.transform.position - bossPos).sqrMagnitude;
                float db = ((Vector2)b.transform.position - bossPos).sqrMagnitude;
                return da.CompareTo(db);
            });


            List<GameObject> deleteList = new(selected.Count);
            foreach (var dagger in selected)
            {
                if (!dagger) continue;
                yield return boss.RetrieveObject(dagger);
                dagger.SetActive(false);
                deleteList.Add(dagger);
            }
            foreach (var d in deleteList) boss.daggerList.Remove(d); 
        }
else
{
    // 대거 리스트의 0번 단검을 기준으로
    if (boss.daggerList.Count > 0)
    {
        var d0 = boss.daggerList[0];
        if (d0 != null && d0.activeInHierarchy)
        {
            Vector2 daggerPos = d0.transform.position;

            // 단검 → 플레이어 방향 기준으로, 플레이어 뒤쪽(같은 선상)에 보스를 배치하면
            // 보스→플레이어, 보스→단검 벡터가 거의 평행 -> 회수 각도 조건 충족
            Vector2 dir = (playerPos - daggerPos).normalized;
            float distFromPlayer = 1.8f; // 필요시 조절
            Vector2 desired = playerPos + dir * distFromPlayer;

            // 장애물/겹침 보정: 선상 앞/뒤 → 좌/우 순으로 스캔
            if (!boss.IsPointFree(desired))
            {
                bool placed = false;

                int alongSamples = 8; float alongStep = 0.35f;
                for (int i = 1; i <= alongSamples && !placed; i++)
                {
                    Vector2 fwd  = playerPos + dir * (distFromPlayer + i * alongStep);
                    if (boss.IsPointFree(fwd)) { desired = fwd; placed = true; break; }

                    Vector2 back = playerPos + dir * (distFromPlayer - i * alongStep);
                    if (boss.IsPointFree(back)) { desired = back; placed = true; break; }
                }

                if (!placed)
                {
                    Vector2 perp = boss.Perp(dir);
                    int lateralSamples = 6; float lateralStep = 0.3f;
                    for (int i = 1; i <= lateralSamples && !placed; i++)
                    {
                        Vector2 left  = desired + perp * (i * lateralStep);
                        if (boss.IsPointFree(left))  { desired = left;  placed = true; break; }

                        Vector2 right = desired - perp * (i * lateralStep);
                        if (boss.IsPointFree(right)) { desired = right; placed = true; break; }
                    }
                }
            }

            // 보스 이동 (DOTween 기반 Move 사용)
            Vector2 moveDir = (desired - (Vector2)boss.transform.position);
            float moveDist = moveDir.magnitude;
            if (moveDist > 0.01f)
                yield return boss.Move(moveDir.normalized, moveDist);

            // 도착 후 회수
            yield return boss.RetrieveObject(d0);
            d0.SetActive(false);
            boss.daggerList.RemoveAt(0);
        }
    }
}

        boss.ChangeState(new PostPatternState(PostDelay){ mob = boss });
    }

    // (선택) 가중치 계산 등에 쓰고 싶으면
    private int CountDaggersWithinAngle(float maxAngle)
    {
        var bossPos   = (Vector2)NagaRogue.transform.position;
        var playerPos = (Vector2)SceneContext.Character.transform.position;
        Vector2 forward = (playerPos - bossPos).normalized;

        int cnt = 0;
        foreach (var d in NagaRogue.daggerList)
        {
            if (d == null || !d.activeInHierarchy) continue;
            Vector2 toDagger = (Vector2)d.transform.position - bossPos;
            if (Vector2.Dot(toDagger, forward) <= 0f) continue;
            if (Vector2.Angle(forward, toDagger) <= maxAngle) cnt++;
        }
        return cnt;
    }
}

        public class BoomerangShuriken: NagaRogueState
        {
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer > NagaRogue.MeleeAttackRange) ? 100 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.CurveThrow(NagaRogue.shurikenPrefab, curveAmount:4f, throwSpeed:4f);
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }

        public class PostPatternState : NagaRogueState
        {
            private readonly float _delay;
            private const float RageDelayPercent = 0.7f;

            private float Delay
            {
                get
                {
                    if (NagaRogue.currentHp <= NagaRogue.maxHp * NagaRogue._phaseChangeHealthPercent) return _delay * RageDelayPercent;
                    return _delay;
                }
            }
            
            public PostPatternState(float delay)
            {
                _delay = delay;
            }

            public PostPatternState() {}
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                if (NagaRogue.currentHp <= NagaRogue.maxHp * NagaRogue._phaseChangeHealthPercent)
                {
                    yield return NagaRogue.Move(NagaRogue.DirectionToPlayer);
                    yield return new WaitForSeconds(0.2f);
                    yield return NagaRogue.ThrustAttack(NagaRogue.thrustAttackPrefab);
                    yield return new WaitForSeconds(0.3f);
                }
                yield return NagaRogue.SetIdleAnim(Delay);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }
        
        public class DetectingPlayer : NagaRogueState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Debug.Log("Detecting Player");
                while (true)
                {
                    Transform target = NagaRogue._detector.DetectTarget()?.transform;
                    if (target != null)
                    {
                        mob.ChangeState();
                        yield break;
                    }
                    yield return new WaitForSeconds(1f);
                }
            }
        }
        
        //------------------------------------------------
        public class LastDitchPattern : NagaRogueState
        {

            private float camShakeTime = 1f;
            public override int GetWeight()
            {
                return (NagaRogue.currentHp <= NagaRogue.maxHp * NagaRogue._lastDitchHealthPercent && NagaRogue.canLastDitch)? 999999 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                NagaRogue.canLastDitch = false;
                yield return new OpenChatEvent().Execute();
                yield return new ShowMessages(NagaRogue.phaseChangeChat, 3f).Execute();
                yield return new CloseChatEvent().Execute();
                yield return new SetKeyInputEvent(){_isEnable = false}.Execute();
                yield return new SetKeyInputEvent(){_isEnable = true}.Execute();
                
                float t = 0;
                while (true)
                {
                    if (t >= camShakeTime) break;
                    t += Time.deltaTime;
                    SceneContext.CameraManager.CameraShake(0.2f);
                }
                
                NagaRogue.sandstormPrefab.gameObject.SetActive(true);
                var module = NagaRogue.sandstormPrefab.velocityOverLifetime;
                NagaRogue.StartCoroutine(IncrementOrbital(module.orbitalYMultiplier)); 
                yield return NagaRogue.Teleport(NagaRogue.centerPoint.position);

                for (int i = 0; i < 8; i++)
                {
                    Sound.Play("ENEMY_NagaRogue_Ambush");
                    NagaRogue.GetFreePosInCircle(SceneContext.Character.transform.position, 2f, out var pos);
                    yield return NagaRogue.Teleport(pos, 2f);
                    yield return new WaitForSeconds(0.2f);
                    yield return NagaRogue.ThrustAttack(NagaRogue.thrustAttackPrefab);
                    yield return new WaitForSeconds(0.3f);
                }
                
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }

            public IEnumerator IncrementOrbital(float orbitSpeed)
            {
                while (orbitSpeed < 2)
                {
                    yield return new WaitForSeconds(0.1f);
                    orbitSpeed = Mathf.Clamp(orbitSpeed+0.01f, 1, 2);
                }
            }
        }
    
    }
}