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
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaRogue
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

        private List<GameObject> daggerList = new();
        public float curveThrowRnage = 10f;
        public float phaseChangeHealthPercent = 0.5f;
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
            maxHp = 30;
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
    
        private IEnumerator MeleeAttack(GameObject prefab, float distance = 0.5f, float duration = 0.5f)
        {
            FaceToPlayer();
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Stab_{dir}");
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
            Direction dir = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Throw_{dir}");
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

        private IEnumerator RetrieveObject(GameObject prefab, float retrieveSpeed = 30f)
        {
            FaceToPlayer();
            Vector3 dirToPlayer = SceneContext.Character.transform.position - prefab.transform.position;
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
        private IEnumerator RangeFanThrow(GameObject prefab, int daggerCount = 4, float spreadAngle = 45f, float throwSpeed = 5f)
        {
            Sound.Play("ENEMY_NagaRogue_Throw");
            FaceToPlayer();
            Direction direction = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Throw_{direction}",-1,0f);

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
                Vector3 spawnPos = origin + dir * 0.5f;
                Vector3 targetPos = spawnPos + dir * 10f; 

                AttackObjectController.Create(
                    spawnPos,
                    dir,
                    prefab,
                    new LinearMovement(spawnPos, targetPos, throwSpeed)
                ).StartRoutine();
            }
            yield return new WaitForSeconds(0.2f);
        }
        private IEnumerator RangeFanDaggerThrow(GameObject prefab, int daggerCount = 4, float spreadAngle = 45f, float throwSpeed = 20f, float daggerRange = 10f)
        {
            FaceToPlayer();
            Direction direction = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Throw_{direction}",-1,0f);

            Vector3 origin = transform.position;
            Vector3 toPlayer = (SceneContext.Character.transform.position - origin).normalized;
            float daggerSpawnDistance = 0.5f;
        
            float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

            float startAngle = baseAngle - (spreadAngle / 2f);
            float angleStep = spreadAngle / (daggerCount - 1);

            for (int i = 0; i < daggerCount; i++)
            {
                float angle = startAngle + angleStep * i;
                float rad = angle * Mathf.Deg2Rad;

                Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0).normalized;
                Vector3 spawnPos = origin + dir * daggerSpawnDistance;
                Vector3 targetPos = spawnPos + dir * daggerRange;

                AttackObjectController ac = AttackObjectController.Create(
                    spawnPos,
                    dir,
                    prefab,
                    new LinearMovement(spawnPos, targetPos, throwSpeed)
                );
                ac.gameObject.GetComponent<ReturnDagger>().Initiate(transform);
                ac.StartRoutineWithCallOnEnd(() => daggerList.Add(ac.gameObject));
                Sound.Play("ENEMY_NagaRogue_Throw");
                yield return new WaitForSeconds(0.02f);
            }
        }
        public IEnumerator Move(Vector3 dir, float distance = 2f, float duration = 0.5f)
        {
            // 1) 애니메이션
            Direction direction = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            Animator.Play($"Dash_{direction}");

            // 2) 캐릭터 콜라이더 크기만큼 BoxCast 해서 충돌 체크
            Collider2D col = GetComponent<Collider2D>();
            RaycastHit2D hit = Physics2D.BoxCast(
                col.bounds.center,
                col.bounds.size,
                0f,
                dir,
                distance,
                LayerMask.GetMask("Obstacle")
            );

            // 충돌 지점까지 거리, 없으면 원거리 이동
            float moveDist = hit.collider != null ? hit.distance : distance;

            // 3) 실제 이동
            Vector3 startPos = transform.position;
            Vector3 endPos   = startPos + dir * moveDist;

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
        private bool TryFindTeleportSpotOnRing(
            Vector3 center, float radius, out Vector3 found,
            int angleSamples = 32,              // 각도 샘플 수
            int radialSteps = 4, float radialStep = 0.5f, // 반지름 줄이며 재시도
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
            float baseAng = Mathf.Atan2((transform.position - center).y, (transform.position - center).x);
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

        public IEnumerator Teleport(Vector3 pos, float distance = 2f)
        {
            FaceToPlayer();
            Direction direction = NagaRogueAction.GetDirectionToTarget(DirectionToPlayer);
            _animator.Play($"Dash_{direction}",-1,0f);
            yield return ChangeAlpha(0f);
            Vector3 dst;
            bool ok = TryFindTeleportSpotOnRing(
                pos, distance, out dst,
                angleSamples: 32,    // 필요하면 24~48로 조절
                radialSteps: 4,      // 못 찾으면 반지름을 최대 4번 줄여 재시도
                radialStep: 0.5f,    // 0.5 유닛씩 안쪽으로
                skin: 0.04f
            );
            if (!ok)
            {
                dst = SceneContext.Character.transform.position;
            }
            transform.position = dst;
            yield return new WaitForSeconds(0.2f);
            FaceToPlayer();
            yield return ChangeAlpha(1f);
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

        public class DashSlash : NagaRogueState
        {
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.MeleeAttackRange) ? 50 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Sound.Play("ENEMY_NagaRogue_Ambush");
                yield return NagaRogue.Move(NagaRogue.DirectionToPlayer);
                yield return NagaRogue.MeleeAttack(NagaRogue.basicAttackPrefab, 1f);
                yield return NagaRogue.MeleeAttack(NagaRogue.basicAttackPrefab, 1f);
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }

        public class DaggerFan : NagaRogueState
        {
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.RangedAttackRange && mob.DistanceToPlayer > mob.MeleeAttackRange) ? 50 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.RangeFanDaggerThrow(NagaRogue.daggerPrefab, 4, 180f);
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }
        public class DaggerFlower : NagaRogueState
        {
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.RangedAttackRange && mob.DistanceToPlayer > mob.MeleeAttackRange && NagaRogue.daggerList.Count <= 10) ? 100 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.RangeFanDaggerThrow(NagaRogue.daggerPrefab, 15, 360f);
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }    
        public class DaggerRetrieve : NagaRogueState
        {
            public override int GetWeight()
            {
                return (NagaRogue.daggerList.Count * 10);
            }

            public override IEnumerator StateCoroutine()
            {
                List<GameObject> deleteDaggerList = new();
                yield return NagaRogue.Move(-NagaRogue.DirectionToPlayer);
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
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }
        public class TeleportDaggerRetrieve : NagaRogueState
        {
            public override int GetWeight()
            {
                return (NagaRogue.daggerList.Count >= 10 ? 300 : 0);
            }

            public override IEnumerator StateCoroutine()
            {
                List<GameObject> deleteDaggerList = new();
                yield return NagaRogue.Teleport(SceneContext.Character.transform.position);
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
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }
        public class ThrowBackstep : NagaRogueState
        {
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer <= mob.MeleeAttackRange) ? 100 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.RangeFanDaggerThrow(NagaRogue.daggerPrefab,3, 90f);
                yield return NagaRogue.Move(-NagaRogue.DirectionToPlayer);
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }

        public class ThrowCurveShuriken : NagaRogueState
        {
        
            public override int GetWeight()
            {
                return (mob.DistanceToPlayer > NagaRogue.RangedAttackRange) ? 50 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return NagaRogue.CurveThrow(NagaRogue.shurikenPrefab, curveAmount:NagaRogue.GetRandomCurve());
                yield return NagaRogue.CurveThrow(NagaRogue.shurikenPrefab, curveAmount:NagaRogue.GetRandomCurve());
                yield return NagaRogue.CurveThrow(NagaRogue.shurikenPrefab, curveAmount:NagaRogue.GetRandomCurve());
                yield return NagaRogue.CurveThrow(NagaRogue.shurikenPrefab, curveAmount:NagaRogue.GetRandomCurve());
                yield return new WaitForSeconds(1f);
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
                mob.Animator.SetBool("isMove", false);
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
                yield return new ShowMessages(NagaRogue.phaseChangeChat, 3f).Execute();
                yield return new CloseChatEvent().Execute();
                yield return new SetKeyInputEvent(){_isEnable = false}.Execute();
                var main = NagaRogue.sandStormParticles.main;
                main.startSpeed = 10f;
                var emission = NagaRogue.sandStormParticles.emission;
                emission.rateOverTime = 200f;            
                NagaRogue.ChangeState();
                yield return new SetKeyInputEvent(){_isEnable = true}.Execute();
                Vector3 originPos = NagaRogue.transform.position;
                yield return NagaRogue.Teleport(new Vector3(999999, 999999, 0));
                // NagaRogue.sandstormPrefab.SetActive(true);
            
                for (int i = 0; i < 3; i++)
                {
                    yield return NagaRogue.CamelAttack();
                }

                yield return NagaRogue.Teleport(originPos);
                Instantiate(NagaRogue.poisonMinionPrefab, NagaRogue.transform.position + new Vector3(2,1,0), Quaternion.identity);
                Instantiate(NagaRogue.sandMinionPrefab, NagaRogue.transform.position + new Vector3(-2,1,0), Quaternion.identity);
                yield return new WaitForSeconds(1f);
                NagaRogue.ChangeState(new DetectingPlayer() {mob = NagaRogue});
            }
        }
    
    }
}