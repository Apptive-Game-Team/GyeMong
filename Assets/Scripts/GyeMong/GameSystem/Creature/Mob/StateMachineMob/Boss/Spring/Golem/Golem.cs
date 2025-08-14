using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Chat;
using GyeMong.EventSystem.Event.CinematicEvent;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Indicator;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.SoundSystem;
using Unity.VisualScripting;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    public class Golem : Boss
    {
        [SerializeField] private RootPatternManger mapPattern;
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject shockwavePrefab;
        [SerializeField] private GameObject pushOutAttackPrefab;
        float attackdelayTime = 1f;
        [SerializeField] private SoundObject _shockwavesoundObject;
        public SoundObject ShockwaveSoundObject => _shockwavesoundObject;
        [SerializeField] private SoundObject _tossSoundObject;

        [Header("Stop Animator")]
        [SerializeField] private Animator targetAnimator;

        [Header("Change Sprite")]
        [SerializeField] private SpriteRenderer targetSpriteRenderer;
        [SerializeField] private Sprite newSprite1;
        
        [SerializeField] private float autoSkipTime = 3f;

        [Header("Boss Room Object")]
        [SerializeField] private GameObject bossRoomObj1;
        [SerializeField] private GameObject bossRoomObj2;

        public SoundObject TossSoundObject => _tossSoundObject;
        protected override void Initialize()
        {
            maxPhase = 2;
            maxHps.Clear();
            maxHps.Add(20f);
            maxHps.Add(30f);
            currentHp = maxHps[currentPhase];
            currentShield = 0f;
            damage = 30f;
            currentShield = 0f;
            detectionRange = 10f;
            maxMeleeAttackRange = 5f;
            minMeleeAttackRange = 3f;
        }

        private Vector3[] GetCirclePoints(Vector3 center, float radius, int numberOfPoints)
        {
            Vector3[] points = new Vector3[numberOfPoints];
            for (int i = 0; i < numberOfPoints; i++)
            {
                float angle = i * Mathf.PI * 2 / numberOfPoints;
                float x = center.x + Mathf.Cos(angle) * radius;
                float y = center.y + Mathf.Sin(angle) * radius;
                points[i] = new Vector3(x, y, 0);
            }
            return points;
        }

        public IEnumerator MakeShockwave()
        {
            float targetRadius = 18;
            int startRadius = 4;
            Vector3 dirToPlayer = SceneContext.Character.transform.position - transform.position;
            float playerAngle = Mathf.Atan2(dirToPlayer.y, dirToPlayer.x) * Mathf.Rad2Deg;
            if (playerAngle < 0f) playerAngle += 360f;
            float sectorMin = playerAngle - 45f;
            float sectorMax = playerAngle + 45f;
            if (sectorMin < 0f) sectorMin += 360f;
            if (sectorMax >= 360f) sectorMax -= 360f;
            float excludeCenter = sectorMin + Random.Range(0f, 30f);
            if (excludeCenter >= 360f) excludeCenter -= 360f;

            float excludeMin = excludeCenter - 10f;
            float excludeMax = excludeCenter + 10f;

            if (excludeMin < 0f) excludeMin += 360f;
            if (excludeMax >= 360f) excludeMax -= 360f;

            for (int i = startRadius; i <= targetRadius; i++)
            {
                Vector3[] points = GetCirclePoints(transform.position, i, i * 3 + 10);
                ShockwaveSoundObject.SetSoundSourceByName("ENEMY_Shockwave");
                StartCoroutine(ShockwaveSoundObject.Play());
                SceneContext.CameraManager.CameraShake(0.2f);

                foreach (Vector3 point in points)
                {
                    Vector3 dir = (point - transform.position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    if (angle < 0f) angle += 360f;
                    bool isInExclude;
                    if (excludeMin < excludeMax)
                        isInExclude = (angle >= excludeMin && angle <= excludeMax);
                    else
                        isInExclude = (angle >= excludeMin || angle <= excludeMax);

                    if (isInExclude)
                        continue;
                    StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator(
                        shockwavePrefab,
                        point,
                        Quaternion.identity,
                        attackdelayTime / 2,
                        () => AttackObjectController.Create(
                            point,
                            Vector3.zero,
                            shockwavePrefab,
                            new StaticMovement(point, attackdelayTime / 2)
                        ).StartRoutine()));
                }

                yield return new WaitForSeconds(attackdelayTime / 3);
            }
        }



        public IEnumerator MakeShock()
        {
            int targetRadius = 4;
            Vector3[] points = GetCirclePoints(transform.position, targetRadius, targetRadius * 3 + 10);
            foreach (Vector3 point in points)
            {
                StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator
                    (shockwavePrefab,point, Quaternion.identity, attackdelayTime / 2,
                        () =>
                        {
                            ShockwaveSoundObject.SetSoundSourceByName("ENEMY_Shockwave");
                            StartCoroutine(ShockwaveSoundObject.Play());
                            SceneContext.CameraManager.CameraShake(0.2f);
                            AttackObjectController.Create(
                                point,
                                Vector3.zero,
                                shockwavePrefab,
                                new StaticMovement(
                                    point,
                                    attackdelayTime / 2)
                            ).StartRoutine();
                        }));
            }
            
            yield return new WaitForSeconds(attackdelayTime / 2);
        }
        public abstract class GolemState : CoolDownState
        {
            public Golem Golem => mob as Golem;
            protected Dictionary<System.Type, int> weights;
            protected virtual void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                {
                    { typeof(MeleeAttack), (Golem.DistanceToPlayer <= Golem.maxMeleeAttackRange && Golem.DistanceToPlayer >= Golem.minMeleeAttackRange) ? 20 : 0 },
                    { typeof(FallingCubeAttack), 5 },
                    { typeof(ChargeShield), 50 },
                    { typeof(UpStoneAttack), (Golem.DistanceToPlayer >= Golem.maxMeleeAttackRange) ? 10 : 0 },
                    { typeof(ShockwaveAttack), (Golem.CurrentPhase == 1) ? 5 : 0 },
                    { typeof(PushOutAttack), (Golem.DistanceToPlayer <= Golem.maxMeleeAttackRange) ? 10 : 0 }
                };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
            protected Dictionary<System.Type, int> NextStateWeights
            {
                get
                {
                    return weights;
                }
            }
        }

        public class MeleeAttack : GolemState
        {
            public override int GetWeight()
            {
                return (Golem.DistanceToPlayer < Golem.maxMeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("TwoHand", true);
                yield return Golem.MakeShock();
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);
                Golem.Animator.SetBool("TwoHand", false);
                SetWeights();
                Golem.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                {
                    { typeof(FallingCubeAttack), 5 },
                    { typeof(ChargeShield), 50 },
                    { typeof(UpStoneAttack), (Golem.DistanceToPlayer >= Golem.maxMeleeAttackRange) ? 10 : 0 },
                    { typeof(ShockwaveAttack), (Golem.CurrentPhase == 1) ? 5 : 0 },
                    { typeof(PushOutAttack), (Golem.DistanceToPlayer <= Golem.maxMeleeAttackRange) ? 10 : 0 }
                };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class PushOutAttack : GolemState
        {
            public override int GetWeight()
            {
                return (Golem.DistanceToPlayer < Golem.maxMeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("Push", true);
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);
                SceneContext.CameraManager.CameraShake(0.15f);
                Vector3 targetPos = SceneContext.Character.transform.position;
                Golem.StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator
                    (Golem.pushOutAttackPrefab, targetPos - Golem.DirectionToPlayer * 0.5f, Quaternion.identity, Golem.attackdelayTime / 2,
                        () => AttackObjectController.Create(
                    targetPos - Golem.DirectionToPlayer * 0.5f,
                    Vector3.zero,
                    Golem.pushOutAttackPrefab,
                    new StaticMovement(
                        targetPos - Golem.DirectionToPlayer * 0.5f,
                        Golem.attackdelayTime / 2)
                    )
                    .StartRoutine()));
                yield return new WaitForSeconds(Golem.attackdelayTime);
                Golem.Animator.SetBool("Push", false);
                SetWeights();
                Golem.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                {
                    { typeof(MeleeAttack), (Golem.DistanceToPlayer <= Golem.maxMeleeAttackRange && Golem.DistanceToPlayer >= Golem.minMeleeAttackRange) ? 20 : 0 },
                    { typeof(FallingCubeAttack), 5 },
                    { typeof(ChargeShield), 50 },
                    { typeof(UpStoneAttack), (Golem.DistanceToPlayer >= Golem.maxMeleeAttackRange) ? 10 : 0 },
                    { typeof(ShockwaveAttack), (Golem.CurrentPhase == 1) ? 5 : 0 }
                };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }

        public class FallingCubeAttack : GolemState
        {
            public override int GetWeight()
            {
                return 5;
            }
            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("Toss", true);
                Golem.StartCoroutine(Golem.TossSoundObject.Play());
                yield return new WaitForSeconds(Golem.attackdelayTime * 2);
                GameObject cube = Instantiate(Golem.cubePrefab, SceneContext.Character.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
                Golem.Animator.SetBool("Toss", false);
                SetWeights();
                Golem.ChangeState(NextStateWeights);
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                Golem.Animator.SetBool("Toss", false);
            }
        }

        public class ChargeShield : GolemState
        {
            public ChargeShield()
            {
                cooldownTime = 30f;
            }
            public override int GetWeight()
            {
                return 5;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(Golem.attackdelayTime);
                Golem.currentShield = 5f;
                Golem.MaterialController.SetMaterial(MaterialController.MaterialType.SHIELD);
                Golem.MaterialController.SetFloat(1);
                SetWeights();
                Golem.ChangeState(NextStateWeights);
            }
        }

        public class UpStoneAttack : GolemState
        {
            public override int GetWeight()
            {
                return 5;
            }

            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("OneHand", true);
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);

                int numberOfObjects = 5;
                float interval = 0.2f;
                float fixedDistance = 7f;

                Vector3 direction = Golem.DirectionToPlayer;
                Vector3 spawnStoneRadius = 2 * direction;
                Vector3 startPosition = Golem.transform.position + spawnStoneRadius;

                Golem.StartCoroutine(SpawnFloor(startPosition, direction, fixedDistance, numberOfObjects, interval));

                Golem.Animator.SetBool("OneHand", false);

                yield return new WaitForSeconds(Golem.attackdelayTime );
                SetWeights();
                Golem.ChangeState(NextStateWeights);
            }

            private IEnumerator SpawnFloor(Vector3 startPosition, Vector3 direction, float fixedDistance, int numberOfObjects, float interval)
            {
                for (int i = 0; i <= numberOfObjects; i++)
                {
                    Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
                    AttackObjectController.Create(
                    spawnPosition,
                    Vector3.zero,
                    Golem.floorPrefab,
                    new StaticMovement(
                        spawnPosition,
                        Golem.attackdelayTime * 2)
                    )
                    .StartRoutine();
                    Golem._shockwavesoundObject.SetSoundSourceByName("ENEMY_Shockwave");
                    Golem.StartCoroutine(Golem._shockwavesoundObject.Play());
                    SceneContext.CameraManager.CameraShake(0.1f);
                    yield return new WaitForSeconds(interval);
                }
            }
        }

        public class ShockwaveAttack : GolemState
        {
            public override int GetWeight()
            {
                return (Golem.CurrentPhase == 1) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("TwoHand", true);
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);
                yield return Golem.MakeShockwave();
                Golem.Animator.SetBool("TwoHand", false);
                yield return new WaitForSeconds(Golem.attackdelayTime / 3);
                SetWeights();
                Golem.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                {
                    { typeof(MeleeAttack), (Golem.DistanceToPlayer <= Golem.maxMeleeAttackRange && Golem.DistanceToPlayer >= Golem.minMeleeAttackRange) ? 20 : 0 },
                    { typeof(FallingCubeAttack), 5 },
                    { typeof(ChargeShield), 50 },
                    { typeof(UpStoneAttack), (Golem.DistanceToPlayer >= Golem.maxMeleeAttackRange) ? 10 : 0 },
                    { typeof(PushOutAttack), (Golem.DistanceToPlayer <= Golem.maxMeleeAttackRange) ? 10 : 0 }
                };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        protected override void TransPhase()
        {
            if (currentPhase < maxHps.Count - 1)
            {
                currentPhase++;
                StopAllCoroutines();
                currentState.OnStateExit();
                Animator.SetBool("isStun", false);
                MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
                StartCoroutine(ChangingPhase());
            }
            else
            {
                MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
                Die();
            }
        }
        protected override void Die()
        {
            base.Die();
            Animator.SetBool("isDown", true);
            mapPattern.DeActivateRootObjects();
            StartCoroutine(DieRoutine());
        }
        private IEnumerator DieRoutine()
        {
            yield return StartCoroutine(DownTrigger());
            StageManager.ClearStage(this);
        }

        public IEnumerator DownTrigger()
        {
            return TriggerEvents();
        }

        private IEnumerator TriggerEvents()
        {
            var changeSpriteEvent = new ChangeSpriteEvent();
            changeSpriteEvent.SetSpriteRenderer(targetSpriteRenderer);
            changeSpriteEvent.SetSprite(newSprite1);
            yield return changeSpriteEvent.Execute();

            yield return new HideBossHealthBarEvent().Execute();
            
            SceneContext.CameraManager.CameraFollow(GameObject.FindGameObjectWithTag("Player").transform);

            var zoomEvent = new CameraZoomInOut();
            zoomEvent.SetSize(3.6f);
            zoomEvent.SetDuration(0.5f);
            yield return StartCoroutine(zoomEvent.Execute());

            var activateBossRoomEvent = new ActivateBossRoomEvent();

            var deactivateEvent = new DeActivateBossRoomEvent();

            deactivateEvent.SetBossRoomObject(bossRoomObj1);
            yield return deactivateEvent.Execute();

            deactivateEvent.SetBossRoomObject(bossRoomObj2);
            yield return deactivateEvent.Execute();
        }

        public override IEnumerator Stun(float duration)
        {
            Debug.Log("Check1");
            currentShield = 0f;
            MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
            Debug.Log("Check2");
            Animator.SetBool("isStun", true);
            currentState.OnStateExit();
            StopCoroutine(_currentStateCoroutine);
            yield return new WaitForSeconds(duration);
            Animator.SetBool("isStun", false);
            ChangeState();
        }
    }
}