using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator;
using GyeMong.GameSystem.Map.Stage;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaWarrior
{
    public class NagaWarrior : Boss
    {
        [SerializeField] private GameObject meleeAttackPrefab;
        [SerializeField] private GameObject breathPrefab;
        [SerializeField] private SkllIndicatorDrawer SkillIndicator;
        float attackdelayTime = 1f;
        [SerializeField] private DailyCycleManager dailyCycleManager;
        bool isOverheat = false;
        bool isCool = false;

        private Color dawnColor = new Color32(255, 255, 255, 255);
        private Color dayColor = new Color32(255, 85, 85, 255);
        private Color duskColor = new Color32(255, 255, 255, 255);

        protected override void Initialize()
        {
            maxHp = 200f;
            currentHp = maxHp;
            damage = 10f;
            speed = 2f;
            currentShield = 0f;
            detectionRange = 10f;
            MeleeAttackRange = 2f;
            RangedAttackRange = 8f;
            //SkillIndicator = transform.Find("SkillIndicator").GetComponent<SkllIndicatorDrawer>();

            ChangeState();
        }
        private void Update()
        {
            UpdateTime();
        }
        void UpdateTime()
        {
            Color targetColor;
            if (dailyCycleManager.currentTimePercent < 0.25f)
            {
                isCool = false;
                float t = dailyCycleManager.currentTimePercent / 0.25f;
                targetColor = Color.Lerp(dawnColor, dayColor, t);
            }
            else if (dailyCycleManager.currentTimePercent < 0.5f)
            {
                isOverheat = true;
                float t = (dailyCycleManager.currentTimePercent - 0.25f) / 0.25f;
                targetColor = Color.Lerp(dayColor, duskColor, t);
            }
            else if (dailyCycleManager.currentTimePercent > 0.75f)
            {
                isCool = true;
                targetColor = duskColor;
            }
            else
            {
                isOverheat = false;
                targetColor = duskColor;
            }
            SpriteRenderer.color = targetColor;
            if (isOverheat)
            {
                damage = 15f;
                speed = 3f;
            }
            else if(isCool)
            {
                damage = 5f;
                speed = 1f;
            }
            else
            {
                damage = 10f;
                speed = 2f;
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
                        { typeof(MeleeAttack), (NagaWarrior.DistanceToPlayer <= NagaWarrior.MeleeAttackRange) ? 5 : 0 },
                        { typeof(JumpAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.MeleeAttackRange) ? 5 : 0 },
                        { typeof(TaleRushAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.MeleeAttackRange) ? 5 : 0 },
                        { typeof(AuraAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.RangedAttackRange) ? 5 : 0 },
                        { typeof(BreathAttack), (NagaWarrior.DistanceToPlayer >= NagaWarrior.RangedAttackRange) ? 5 : 0 }
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
                return (NagaWarrior.DistanceToPlayer <= NagaWarrior.MeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.DirectionToPlayer);
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.DirectionToPlayer);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
        }
        public class JumpAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.MeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Vector3 targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                yield return NagaWarrior.ParabolaJump(targetPos, height: 4f, duration: 0.5f);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.DirectionToPlayer);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
        }
        public IEnumerator ParabolaJump(Vector3 target, float height, float duration)
        {
            Vector3 start = transform.position;
            float time = 0f;

            while (time < duration)
            {
                float t = time / duration;
                float yOffset = 4 * height * t * (1 - t);
                transform.position = Vector3.Lerp(start, target, t) + Vector3.up * yOffset;
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = target;
        }
        public class TaleRushAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.MeleeAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                yield return NagaWarrior.HalfRushAttack(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.lastRushDirection);
                yield return NagaWarrior.RushAttack(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.lastRushDirection);
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime / 2);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
        }
        public class AuraAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.RangedAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                NagaWarrior.SpawnSkillCollider(NagaWarrior.DirectionToPlayer);
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
        }
        public class BreathAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.RangedAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                yield return NagaWarrior.SpawnBreath(5f, 6);
                SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
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

                    AttackObjectController.Create(
                        spawnPosition,
                        direction,
                        breathPrefab,
                        new StaticMovement(
                            spawnPosition,
                            attackdelayTime*2)
                    )
                    .StartRoutine();
                }
            }
            yield return attackdelayTime*2;
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
        private void SpawnSkillCollider(Vector3 direction)
        {
            Vector2 spawnPosition = transform.position + direction * 1f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject attackCollider = Instantiate(meleeAttackPrefab, spawnPosition, spawnRotation);

            StartCoroutine(MoveColliderForward(attackCollider.transform, direction.normalized, 5f, attackdelayTime * 2));
        }
        private IEnumerator MoveColliderForward(Transform obj, Vector3 direction, float speed, float lifeTime)
        {
            float elapsedTime = 0f;

            while (elapsedTime < lifeTime)
            {
                obj.position += direction * speed * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(obj.gameObject);
        }
        protected override void Die()
        {
            base.Die();
            Animator.SetBool("isDown", true);
            StageManager.ClearStage(this);
        }
        private void SpawnAttackCollider(Vector3 direction)
        {
            Vector2 spawnPosition = transform.position + direction * 1f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
            AttackObjectController.Create(
                    spawnPosition,
                    direction,
                    meleeAttackPrefab,
                    new StaticMovement(
                        spawnPosition,
                        attackdelayTime / 2)
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