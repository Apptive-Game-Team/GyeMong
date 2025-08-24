using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.SoundSystem;
using Unity.Mathematics;
using UnityEngine;
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
        [SerializeField] private GameObject basicAttackPrefab;
        [SerializeField] private GameObject daggerPrefab;
        [SerializeField] private GameObject shurikenPrefab;
        [SerializeField] private GameObject sandstormPrefab;
        [SerializeField] private GameObject rushCamelPrefab;
        [SerializeField] private GameObject poisonMinionPrefab;
        [SerializeField] private GameObject sandMinionPrefab;
        [SerializeField] private ParticleSystem sandStormParticles;
        [SerializeField] private MultiChatMessageData phaseChangeChat;
        [SerializeField] private EdgeCollider2D mapEdge;
        private float boundaryInset = 0.2f; // 안쪽으로 밀어넣는 거리

        private List<GameObject> daggerList = new();
        public const float DaggerRange = 20f;
        public float curveThrowRnage = 10f;
        public float phaseChangeHealthPercent = 0.66f;
        public bool canPhaseChange = true;

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
        }

        private IEnumerator SetIdleAnim(float delay)
        {
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Idle_{dir}",-1,0f);
            yield return new WaitForSeconds(delay);
        }
        private IEnumerator MeleeAttack(GameObject prefab, float duration = 0.5f)
        {
            const float startPosDist = 1f;
            FaceToPlayer();
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Stab_{dir}");
            AttackObjectController.Create(
                    transform.position + DirectionToPlayer * startPosDist, 
                    DirectionToPlayer, 
                    prefab, 
                    new StaticMovement(
                        transform.position + DirectionToPlayer * startPosDist, 
                        duration)
                )
                .StartRoutine();
            yield return new WaitForSeconds(0.2f);
        }

        private IEnumerator RetrieveObject(GameObject prefab, float retrieveSpeed = 30f)
        {
            FaceToPlayer();
            Vector3 dirToPlayer = SceneContext.Character.transform.position - prefab.transform.position;
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
            yield return null;
        }
    
        public float GetRandomCurve()
        {
            float minCurve = 3f;
            float maxCurve = 5f;
            float baseCurve = Random.Range(minCurve, maxCurve);
            float curveAmount = baseCurve * (Random.value < 0.5f ? -1 : 1);

            return curveAmount;
        }
    
        private IEnumerator CurveThrow(GameObject prefab, float distance = 0.5f, float throwSpeed = 8f, float curveAmount = 1f)
        {
            Sound.Play("ENEMY_NagaRogue_Throw");
            FaceToPlayer();
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Throw_{dir}",0,0f);
            AttackObjectController.Create(
                    transform.position + DirectionToPlayer * distance, 
                    DirectionToPlayer, 
                    prefab, 
                    new CurveMovement(
                        transform.position + DirectionToPlayer * distance, 
                        SceneContext.Character.transform.position,
                        throwSpeed,
                        curveAmount)
                )
                .StartRoutine();
            yield return new WaitForSeconds(0.5f);
        }
        
        private IEnumerator RangeThrow(
            GameObject prefab,
            float daggerRange = 20f,
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
            ac.StartRoutineWithCallOnEnd(() => daggerList.Add(ac.gameObject));

            yield return new WaitForSeconds(0.2f);
        }
        
        private IEnumerator DaggerFanThrow(GameObject prefab, int daggerCount = 3, 
            float spreadAngle = 120f,
            float throwSpeed = 20f, float daggerRange = 20f)
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
                ac.StartRoutineWithCallOnEnd(() => daggerList.Add(ac.gameObject));
                yield return new WaitForSeconds(0.015f);
            }
        }
//---------------------------------여러 방향 선택하는 알고리즘들-----------------------------
// 유틸
private Vector3 Perp(Vector3 v) => new Vector3(-v.y, v.x, 0).normalized;

private bool InViewport(Vector3 world, float margin = 0.06f) {
    var cam = Camera.main;
    if (!cam) return true;
    var v = cam.WorldToViewportPoint(world);
    return v.z > 0 && v.x > margin && v.x < 1 - margin && v.y > margin && v.y < 1 - margin;
}
private static Vector3 ClosestPointOnSegment(Vector3 p, Vector3 a, Vector3 b)
{
    Vector3 ab = b - a;
    float t = Vector3.Dot(p - a, ab) / Mathf.Max(ab.sqrMagnitude, 1e-6f);
    t = Mathf.Clamp01(t);
    return a + ab * t;
}

private bool IsEdgeClockwise(EdgeCollider2D edge)
{
    if (!edge || edge.pointCount < 3) return false;
    var pts = edge.points;
    float area2 = 0f;
    for (int i = 0; i < pts.Length; i++)
    {
        Vector2 a = pts[i];
        Vector2 b = pts[(i + 1) % pts.Length];
        area2 += a.x * b.y - b.x * a.y;
    }
    // 음수면 시계방향
    return area2 < 0f;
}

private bool IsInsideEdge(EdgeCollider2D edge, Vector2 worldPoint)
{
    if (!edge || edge.pointCount < 3) return true;
    var pts = edge.points;
    int n = pts.Length, crossings = 0;
    Vector2 p1 = edge.transform.TransformPoint(pts[0]);
    for (int i = 1; i <= n; i++)
    {
        Vector2 p2 = edge.transform.TransformPoint(pts[i % n]);
        // 오른쪽(+x) 레이 교차 카운트
        bool cond = ((p1.y > worldPoint.y) != (p2.y > worldPoint.y)) &&
                    (worldPoint.x < (p2.x - p1.x) * (worldPoint.y - p1.y) / (p2.y - p1.y) + p1.x);
        if (cond) crossings++;
        p1 = p2;
    }
    return (crossings & 1) == 1;
}

private Vector3 ClampInsideEdge(EdgeCollider2D edge, Vector3 worldPoint, float inset)
{
    if (!edge) return worldPoint;
    if (IsInsideEdge(edge, worldPoint)) return worldPoint;

    // 1) 가장 가까운 선분에서 최근접점 구하기
    var pts = edge.points;
    int n = pts.Length;
    float bestSqr = float.PositiveInfinity;
    int bestIdx = 0;
    Vector3 bestClosest = worldPoint;

    for (int i = 0; i < n; i++)
    {
        Vector3 a = edge.transform.TransformPoint(pts[i]);
        Vector3 b = edge.transform.TransformPoint(pts[(i + 1) % n]);
        Vector3 cp = ClosestPointOnSegment(worldPoint, a, b);
        float sq = (cp - worldPoint).sqrMagnitude;
        if (sq < bestSqr) { bestSqr = sq; bestClosest = cp; bestIdx = i; }
    }

    // 2) 해당 선분의 '안쪽' 노멀 계산
    Vector3 aw = edge.transform.TransformPoint(pts[bestIdx]);
    Vector3 bw = edge.transform.TransformPoint(pts[(bestIdx + 1) % n]);
    Vector2 seg = (bw - aw);
    Vector2 nLeft = new Vector2(-seg.y, seg.x).normalized;
    bool _edgeClockwise = IsEdgeClockwise(mapEdge);
    Vector2 nInside = _edgeClockwise ? -nLeft : nLeft;

    // 3) 안쪽으로 inset 만큼 밀어넣기 + 장애물 겹침시 약간씩 더 안쪽으로
    Vector3 candidate = bestClosest + (Vector3)(nInside * inset);
    for (int t = 0; t < 5 && !IsPointFree(candidate); t++)
        candidate += (Vector3)(nInside * (inset * 0.5f));

    // 4) 여전히 겹치면 접선 방향으로 슬라이드
    if (!IsPointFree(candidate))
    {
        Vector2 tangent = seg.normalized;
        for (int s = 1; s <= 4; s++)
        {
            Vector3 left = candidate + (Vector3)(tangent * s * 0.3f);
            if (IsInsideEdge(edge, left) && IsPointFree(left)) { candidate = left; break; }

            Vector3 right = candidate - (Vector3)(tangent * s * 0.3f);
            if (IsInsideEdge(edge, right) && IsPointFree(right)) { candidate = right; break; }
        }
    }
    return candidate;
}

// 다방향 샘플로 최적 스트레이프 방향 선택
private Vector3 ChooseBestStrafeDir(
    float maxDist = 1.6f,
    int samplesPerSide = 7,
    float coneHalfDeg = 80f)
{
    var col = GetComponent<Collider2D>();
    var center = col.bounds.center;
    var size   = col.bounds.size;

    Vector3 fwd  = DirectionToPlayer.normalized;
    Vector3 side = Perp(fwd);               // 오른쪽
    Vector3[] bases = { side, -side };      // 좌/우 모두 조사

    int obstacleMask = LayerMask.GetMask("Obstacle");

    float bestScore = float.NegativeInfinity;
    Vector3 bestDir = side;
    float bestClr   = 0.3f;

    foreach (var basis in bases)
    {
        for (int i = 0; i < samplesPerSide; i++)
        {
            // [-1..1] 분포로 각도 뽑기 (가운데 → 바깥 순)
            float t = (samplesPerSide == 1) ? 0f : (i / (samplesPerSide - 1f)) * 2f - 1f;
            float ang = t * coneHalfDeg;
            Vector3 cand = Quaternion.Euler(0, 0, ang) * basis;

            // 장애물까지 여유거리 측정(BoxCast)
            var hit = Physics2D.BoxCast(center, size, 0f, cand, maxDist, obstacleMask);
            float clr = hit.collider ? hit.distance : maxDist;

            // 그 방향으로 실제 이동했을 때의 목표점(화면 안 선호)
            Vector3 target = transform.position + cand * Mathf.Min(clr, maxDist);

            // 점수 구성
            float alignStrafe = Mathf.Abs(Vector3.Dot(cand, side)); // 진정한 좌/우일수록 +
            float centerBias  = 0f;
            if (Camera.main) {
                var camC = Camera.main.transform.position; camC.z = transform.position.z;
                float before = Vector3.Distance(transform.position, camC);
                float after  = Vector3.Distance(target, camC);
                centerBias = Mathf.Max(0f, before - after);         // 화면 중앙 쪽으로 가면 +
            }
            float viewPenalty = InViewport(target) ? 0f : 0.6f;      // 화면 밖이면 페널티

            float score = clr + alignStrafe * 0.35f + centerBias * 0.25f - viewPenalty;

            if (score > bestScore)
            {
                bestScore = score;
                bestDir = cand;
                bestClr = clr;
            }
        }
    }
    return bestDir;
}

private IEnumerator PreThrowStrafe(float duration = 0.5f, float distance = 2f)
{
    Vector3 dir= ChooseBestStrafeDir(maxDist : 10f, samplesPerSide: 9, coneHalfDeg: 85f);
    yield return MoveSmart(dir, distance, duration); 
}

// 단검들의 중심(Centroid)
private Vector3 GetDaggersCentroid()
{
    if (daggerList == null || daggerList.Count == 0) return transform.position;
    Vector3 sum = Vector3.zero; int n = 0;
    foreach (var d in daggerList)
    {
        if (d && d.activeInHierarchy) { sum += d.transform.position; n++; }
    }
    return (n > 0) ? sum / n : transform.position;
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
    Vector3 c = GetDaggersCentroid();                          // 단검들의 중심
    Vector3 dir = (p - c);
    if (dir.sqrMagnitude < 0.0001f) dir = DirectionToPlayer;   // 특수 케이스 처리
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

public IEnumerator MoveSmart(Vector3 desiredDir, float distance = 2f, float duration = 0.5f)
{
    // 1) 후보 중 최적 방향 선택
    Vector3 dir = ChooseDashDirection(desiredDir, distance);

    // 2) 히트 시 살짝 ‘슬라이드’: 표면 법선에 직교 방향으로 보정
    Collider2D col = GetComponent<Collider2D>();
    var hit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, dir, distance, LayerMask.GetMask("Obstacle"));

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

    Direction animDir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
    Animator.Play($"Backstep_{animDir}",-1,0f);
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
    
        public IEnumerator CamelAttack(float distance = 10f, float rushSpeed = 10f)
        {
            Sound.Play("ENEMY_NagaRogue_CamelCharge");
            float angle = Random.Range(0f, 360f);
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0).normalized;
        
            Vector3 startPos = SceneContext.Character.transform.position + dir * distance;
            Vector3 endPos = SceneContext.Character.transform.position - dir * distance;
        
            Vector3 perpendicular = new Vector3(-dir.y, dir.x, 0);
            float spacing = 1.2f;

            for (int i = 0; i < 3; i++)
            {
                int offset = i - 1;
                Vector3 offsetVec = perpendicular * (spacing * offset);

                Vector3 spawnPos = startPos + offsetVec;
                Vector3 targetPos = endPos + offsetVec;
            
                AttackObjectController.Create(
                        startPos, 
                        dir,
                        rushCamelPrefab,
                        new LinearMovement(
                            spawnPos, 
                            targetPos,
                            rushSpeed)
                    )
                    .StartRoutine();
            }
            yield return new WaitForSeconds(4f);
        }
    
        private void FaceToPlayer()
        {
            Animator.SetFloat("xDir", DirectionToPlayer.x);
            Animator.SetFloat("yDir", DirectionToPlayer.y);
        }

        //states============================================================================추후 분리
        public class DashSlash : NagaRogueState
        {
            public const float DashRange = 1f;
            private const float MeleeDelay = 0.3f;
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.MeleeAttackRange) ? 50 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Sound.Play("ENEMY_NagaRogue_Ambush");
                yield return NagaRogue.MoveSmart(NagaRogue.DirectionToPlayer, DashRange);
                yield return new WaitForSeconds(MeleeDelay);
                for (int i = 0; i < 2; i++)
                {
                    yield return NagaRogue.MeleeAttack(NagaRogue.basicAttackPrefab);  
                }
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }
        public class TripleDaggerThrow : NagaRogueState
        {
            private const float ThrowDelay = 0.2f;
            private const float PostDelay = 1f;
            private const float ThrowSpeed = 15f;
            private const float AimErrorDeg = 15f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.RangedAttackRange && mob.DistanceToPlayer > mob.MeleeAttackRange) ? 50 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.PreThrowStrafe();
                for (int i = 0; i < 3; i++)
                {
                    if (mob.DistanceToPlayer < mob.MeleeAttackRange)
                    {
                        NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
                        yield break;
                    }
                    yield return NagaRogue.RangeThrow(NagaRogue.daggerPrefab, DaggerRange, ThrowSpeed, AimErrorDeg);
                    yield return new WaitForSeconds(ThrowDelay);
                }
                
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }

        public class KitingThrow : NagaRogueState
        {
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.MeleeAttackRange) ? 100 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                NagaRogue.GetFreePosInCircle(SceneContext.Character.transform.position, 5f, out var pos);
                yield return NagaRogue.Teleport(pos, 7f);
                yield return NagaRogue.DaggerFanThrow(NagaRogue.daggerPrefab, 5, daggerRange: DaggerRange);
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }
        
        public class DaggerFan : NagaRogueState
        {
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return 0;
                return (mob.DistanceToPlayer <= mob.RangedAttackRange && mob.DistanceToPlayer > mob.MeleeAttackRange) ? 50 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.DaggerFanThrow(NagaRogue.daggerPrefab, 3, 60f);
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }
        public class DaggerFlower : NagaRogueState
        {
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return 0;
                return (mob.DistanceToPlayer <= mob.RangedAttackRange && mob.DistanceToPlayer > mob.MeleeAttackRange && NagaRogue.daggerList.Count <= 10) ? 100 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.DaggerFanThrow(NagaRogue.daggerPrefab, 9, 360f);
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }    
        public class TeleportDaggerRetrieve : NagaRogueState
        {
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return (NagaRogue.daggerList.Count >= 10 ? 300 : 0);
            }

            public override IEnumerator StateCoroutine()
            {
                List<GameObject> deleteDaggerList = new();
                Vector3 pos = NagaRogue.FindRetrievePoint(5f);
                yield return NagaRogue.Teleport(pos);
                foreach (var dagger in NagaRogue.daggerList)
                {
                    yield return NagaRogue.RetrieveObject(dagger);
                    dagger.SetActive(false);
                    deleteDaggerList.Add(dagger);
                }

                foreach (var dagger in deleteDaggerList)
                {
                    NagaRogue.daggerList.Remove(dagger);
                }
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }

        public class ThrowShurikenTP : NagaRogueState
        {
            private const float PostDelay = 1f;
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer >= NagaRogue.RangedAttackRange) ? 100 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                for (int i = 0; i < 4; i++)
                {
                    yield return NagaRogue.CurveThrow(NagaRogue.shurikenPrefab, curveAmount:NagaRogue.GetRandomCurve());
                }
                NagaRogue.GetFreePosInCircle(SceneContext.Character.transform.position, 5f, out var pos);
                yield return NagaRogue.Teleport(pos, 5f);
                NagaRogue.ChangeState(new PostPatternState(PostDelay){mob = NagaRogue});
            }
        }

        public class PostPatternState : NagaRogueState
        {
            private readonly float Delay;
            
            public PostPatternState(float delay)
            {
                Delay = delay;
            }

            public PostPatternState() {}
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                if (NagaRogue.currentHp <= NagaRogue.maxHp * NagaRogue.phaseChangeHealthPercent)
                {
                    yield return NagaRogue.DaggerFanThrow(NagaRogue.daggerPrefab, spreadAngle:90f);
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
        public class CallSandStorm : NagaRogueState
        {
            public override int GetWeight()
            {
                return (NagaRogue.currentHp <= NagaRogue.maxHp * NagaRogue.phaseChangeHealthPercent && NagaRogue.canPhaseChange)? 99999 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                NagaRogue.canPhaseChange = false;
                yield return new OpenChatEvent().Execute();
                SceneContext.CameraManager.CameraShake(0.1f);
                yield return new ShowMessages(NagaRogue.phaseChangeChat, 3f).Execute();
                yield return new CloseChatEvent().Execute();
                yield return new SetKeyInputEvent(){_isEnable = false}.Execute();
                
                NagaRogue.ChangeState();
                yield return new SetKeyInputEvent(){_isEnable = true}.Execute();
                

                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }
    
    }
}