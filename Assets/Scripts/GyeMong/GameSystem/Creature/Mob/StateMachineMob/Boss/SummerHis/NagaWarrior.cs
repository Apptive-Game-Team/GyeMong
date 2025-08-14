using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;
using GyeMong.SoundSystem;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event;
using GyeMong.GameSystem.Indicator;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaWarrior
{
    public class NagaWarrior : Boss
    {
        [SerializeField] private GameObject meleeAttackPrefab;
        [SerializeField] private GameObject comboAttackPrefab;
        [SerializeField] private GameObject overHeatComboAttackPrefab;
        [SerializeField] private GameObject TailRush1Prefab;
        [SerializeField] private GameObject TailRush2Prefab;
        [SerializeField] private GameObject pitCenterPrefab;
        [SerializeField] private GameObject pitBoundaryPrefab;
        [SerializeField] private GameObject breathPrefab;
        [SerializeField] private GameObject skillAttackPrefab;
        [SerializeField] private GameObject overHeatSkillAttackPrefab;
        [SerializeField] private SkllIndicatorDrawer SkillIndicator;
        [SerializeField] private GameObject dailyCycleIndicator;
        private AirborneController airborneController;
        float attackdelayTime;
        [SerializeField] private DailyCycleManager dailyCycleManager;
        bool isOverheat = false;
        bool isCool = false;
        public SoundObject curBGM;

        [SerializeField] private BodyHeatUI bodyHeatUI;
        private float bodyHeat;
        private Color dawnColor = new Color32(255, 255, 255, 255);
        private Color dayColor = new Color32(255, 85, 85, 255);
        private Color duskColor = new Color32(255, 255, 255, 255);

        protected override void Initialize()
        {
            maxPhase = 1;
            maxHps.Clear();
            maxHps.Add(200f);
            currentHp = maxHps[currentPhase];
            damage = 10f;
            speed = 2f;
            currentShield = 0f;
            detectionRange = 10f;
            maxMeleeAttackRange = 2f;
            RangedAttackRange = 8f;
            attackdelayTime = 1f;
            SkillIndicator = transform.Find("SkillIndicator").GetComponent<SkllIndicatorDrawer>();
            airborneController = GetComponent<AirborneController>();
        }
        private void Update()
        {
            UpdatePhaseState();
            UpdateBodyHeat();
            bodyHeatUI.SetHeat(bodyHeat);
        }
        private void UpdatePhaseState()
        {
            float t = dailyCycleManager.currentTimePercent;
            Color targetColor;
            float phaseT;

            if (t < 0.25f)
            {
                isCool = false;
                isOverheat = false;
                phaseT = t / 0.25f;
                targetColor = Color.Lerp(dawnColor, dayColor, phaseT);
            }
            else if (t < 0.5f)
            {
                isCool = false;
                isOverheat = true;
                phaseT = (t - 0.25f) / 0.25f;
                targetColor = Color.Lerp(dayColor, duskColor, phaseT);
            }
            else if (t < 0.75f)
            {
                isCool = false;
                isOverheat = false;
                targetColor = duskColor;
            }
            else
            {
                isCool = true;
                isOverheat = false;
                targetColor = duskColor;
            }

            SpriteRenderer.color = targetColor;

            UpdateCombatStats();
        }
        private void UpdateCombatStats()
        {
            if (isOverheat)
            {
                damage = 15f;
                attackdelayTime = 0.5f;
            }
            else if (isCool)
            {
                damage = 5f;
                attackdelayTime = 2f;
            }
            else
            {
                damage = 10f;
                attackdelayTime = 1f;
            }
        }
        private void UpdateBodyHeat()
        {
            float t = dailyCycleManager.currentTimePercent;

            if (t < 0.25f)
            {
                bodyHeat = Mathf.InverseLerp(0f, 0.25f, t);
            }
            else if (t < 0.5f)
            {
                bodyHeat = 1f;
            }
            else if (t < 0.75f)
            {
                bodyHeat = Mathf.InverseLerp(0.75f, 0.5f, t);
            }
            else
            {
                bodyHeat = 0f;
            }
        }
        public abstract class NagaWarriorState : CoolDownState
        {
            public NagaWarrior NagaWarrior => mob as NagaWarrior;
            protected Dictionary<System.Type, int> weights;
            public override void OnStateUpdate()
            {
            }
            protected virtual void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(MeleeAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(JumpAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(TaleRushAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(AuraAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 },
                        { typeof(BreathAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 }
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
        public class MeleeAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer <= NagaWarrior.maxMeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackComboCollider(NagaWarrior.DirectionToPlayer, 1);
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackComboCollider(NagaWarrior.DirectionToPlayer, 2);
                if(NagaWarrior.isOverheat)
                {
                    yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                    NagaWarrior.SpawnAttackComboCollider(NagaWarrior.DirectionToPlayer, 3);
                    //burn effect
                }
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(JumpAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(TaleRushAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(AuraAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 },
                        { typeof(BreathAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 }
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class JumpAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Vector3 targetPos = SceneContext.Character.transform.position;
                Vector3 targetDir = NagaWarrior.DirectionToPlayer;
                NagaWarrior.SkillIndicator.DrawIndicator(SkllIndicatorDrawer.IndicatorType.Circle, targetPos, SceneContext.Character.transform, NagaWarrior.attackdelayTime/2, NagaWarrior.attackdelayTime / 2, 1f);
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                yield return NagaWarrior.airborneController.AirborneTo(targetPos);
                AttackObjectController.Create(
                    NagaWarrior.transform.position + targetDir,
                    Vector3.zero,
                    NagaWarrior.pitBoundaryPrefab,
                    new StaticMovement(
                        NagaWarrior.transform.position + targetDir,
                        NagaWarrior.attackdelayTime/2)
                )
                .StartRoutine();
                AttackObjectController.Create(
                    NagaWarrior.transform.position + targetDir,
                    Vector3.zero,
                    NagaWarrior.pitCenterPrefab,
                    new StaticMovement(
                        NagaWarrior.transform.position + targetDir,
                        NagaWarrior.attackdelayTime/2)
                )
                .StartRoutine();
                SceneContext.CameraManager.CameraShake(0.3f);
                Sound.Play("ENEMY_Rock_Falled");
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(MeleeAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(TaleRushAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(AuraAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 },
                        { typeof(BreathAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 }
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class TaleRushAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                var f_obj = AttackObjectController.Create(
                    NagaWarrior.transform.position,
                    Vector3.zero,
                    NagaWarrior.TailRush1Prefab,
                    new StaticMovement(
                        NagaWarrior.transform.position,
                        NagaWarrior.attackdelayTime / 2)
                );
                f_obj.StartRoutine();
                f_obj.transform.parent = NagaWarrior.transform;
                yield return NagaWarrior.QuickHalfRushAttack();
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                var s_obj = AttackObjectController.Create(
                    NagaWarrior.transform.position,
                    Vector3.zero,
                    NagaWarrior.TailRush2Prefab,
                    new StaticMovement(
                        NagaWarrior.transform.position,
                        NagaWarrior.attackdelayTime / 2)
                );
                s_obj.StartRoutine();
                s_obj.transform.parent = NagaWarrior.transform;
                yield return NagaWarrior.QuickRushAttack();
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(MeleeAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(JumpAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(AuraAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 },
                        { typeof(BreathAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 }
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class AuraAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                if(NagaWarrior.isOverheat)
                    NagaWarrior.SkillIndicator.DrawIndicator(SkllIndicatorDrawer.IndicatorType.Line, NagaWarrior.transform.position, SceneContext.Character.transform, NagaWarrior.attackdelayTime, NagaWarrior.attackdelayTime / 4, 12f);
                //burn effect
                else
                    NagaWarrior.SkillIndicator.DrawIndicator(SkllIndicatorDrawer.IndicatorType.Line, NagaWarrior.transform.position, SceneContext.Character.transform, NagaWarrior.attackdelayTime, NagaWarrior.attackdelayTime / 4, 6f);
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                NagaWarrior.SpawnSkillCollider(NagaWarrior.DirectionToPlayer, SceneContext.Character.transform.position);
                Sound.Play("EFFECT_Sword_Swing");
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(MeleeAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(JumpAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(TaleRushAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(BreathAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 }
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class BreathAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                //burn effect
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                int numberofPoints = 6;
                if(NagaWarrior.isOverheat)
                    numberofPoints = 12;
                yield return NagaWarrior.StartCoroutine(NagaWarrior.SpawnBreath(5f, numberofPoints));
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(MeleeAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(JumpAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(TaleRushAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.maxMeleeAttackRange) ? 5 : 0 },
                        { typeof(AuraAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.RangedAttackRange) ? 5 : 0 }
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        private IEnumerator SpawnBreath(float radius, int numberOfPoints)
        {
            Vector3 center = transform.position;
            Vector3[] points = GetCirclePoints(center, radius, numberOfPoints);
            foreach (Vector3 point in points)
            {
                int numberOfObjects = 5;
                Vector3 direction = (point - center).normalized;
                float distance = radius;
                for (int i = 0; i <= numberOfObjects; i++)
                {
                    float t = i / (float)numberOfObjects;
                    Vector3 spawnPosition = center + direction * (distance * t);
                    StartCoroutine(IndicatorGenerator.Instance.GenerateIndicator
                    (breathPrefab, spawnPosition, Quaternion.identity, attackdelayTime / 2,
                        () => AttackObjectController.Create(
                        spawnPosition,
                        direction,
                        breathPrefab,
                        new StaticMovement(
                            spawnPosition,
                            attackdelayTime * 2)
                        ).StartRoutine()));
                }
            }
            yield return new WaitForSeconds(attackdelayTime * 2);
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
        private void SpawnSkillCollider(Vector3 direction, Vector3 targetPosition)
        {
            Vector2 spawnPosition = transform.position + direction * 1f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
            GameObject skillAttackObj = skillAttackPrefab;
            if (isOverheat)
            {
                skillAttackObj = overHeatSkillAttackPrefab; 
            }
            GameObject attackCollider = Instantiate(skillAttackObj, spawnPosition, spawnRotation);

            StartCoroutine(MoveColliderToTarget(attackCollider.transform, targetPosition, attackdelayTime * 4));
        }
        private IEnumerator MoveColliderToTarget(Transform obj, Vector3 targetPosition, float speed)
        {
            while (Vector3.Distance(obj.position, targetPosition) > 0.01f)
            {
                obj.position = Vector3.MoveTowards(obj.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            obj.position = targetPosition;
            Destroy(obj.gameObject);
        }
        protected override void Die()
        {
            currentState.OnStateExit();
            StopAllCoroutines();
            Sound.Stop(curBGM);
            Sound.Play("ENEMY_Ground_Crash");
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine((new HideBossHealthBarEvent() { _boss = this }).Execute());
            StartCoroutine((new SetActiveObject()
            {
                _gameObject = dailyCycleIndicator,
                isActive = false
            }).Execute());
        }
        private void SpawnAttackComboCollider(Vector3 direction, int combo)
        {
            Vector2 spawnPosition = transform.position + direction * 1f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
            GameObject meleeAttackObj = meleeAttackPrefab;
            if (combo == 2)
            {
                meleeAttackObj = comboAttackPrefab;
            }
            else if(combo == 3)
            {
                meleeAttackObj = overHeatComboAttackPrefab;
            }
            AttackObjectController.Create(
                    spawnPosition,
                    direction,
                    meleeAttackObj,
                    new StaticMovement(
                        spawnPosition,
                        0.5f)
                )
                .StartRoutine();
        }
        public override IEnumerator Stun(float duration)
        {
            MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
            Animator.SetBool("isStun", true);
            currentState.OnStateExit();
            StopCoroutine(_currentStateCoroutine);
            yield return new WaitForSeconds(duration);
            Animator.SetBool("isStun", false);
            ChangeState();
        }
    }
}