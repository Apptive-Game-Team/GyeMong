using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Creature.Player.Component.Collider;
using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.SoundSystem;
using GyeMong.UISystem.Option.Controller;
using UnityEngine;
using static GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Elf.Elf;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Summer.NagaWarrior
{
    public class NagaWarrior : Boss
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject seedPrefab;
        [SerializeField] private GameObject vinePrefab;
        [SerializeField] private GameObject trunkPrefab;
        [SerializeField] private GameObject meleeAttackPrefab;
        [SerializeField] private SkllIndicatorDrawer SkillIndicator;
        float attackdelayTime = 1f;
        [SerializeField] private SoundObject arrowSoundObject;
        [SerializeField] private SoundObject vineSoundObject;
        [SerializeField] private DailyCycleManager dailyCycleManager;
        bool isoverheat = false;

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
                float t = dailyCycleManager.currentTimePercent / 0.25f;
                targetColor = Color.Lerp(dawnColor, dayColor, t);
            }
            else if (dailyCycleManager.currentTimePercent < 0.5f)
            {
                isoverheat = true;
                float t = (dailyCycleManager.currentTimePercent - 0.25f) / 0.25f;
                targetColor = Color.Lerp(dayColor, duskColor, t);
            }
            else
            {
                isoverheat = false;
                targetColor = duskColor;
            }
            SpriteRenderer.color = targetColor;
            if (isoverheat)
            {
                damage = 15f;
                speed = 3f;
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
            /*protected virtual void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(BackStep), (Elf.DistanceToPlayer <= Elf.RangedAttackRange / 2) ? 5 : 0 },
                        { typeof(RushAndAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 50 : 0 },
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange)  ? 50 : 0 },
                        { typeof(MeleeAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) ? 5 : 0},
                        { typeof(WhipAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) && (Elf.CurrentPhase == 1) ? 50 : 0 },
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }*/
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
                //SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }
            /*protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }*/
        }
        public class JumpAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.MeleeAttackRange) ? 1 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Vector3 targetPos = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                yield return NagaWarrior.ParabolaJump(targetPos, height: 4f, duration: 0.5f);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.DirectionToPlayer);
                //SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }

            /*protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }*/
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
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.MeleeAttackRange) ? 1 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return NagaWarrior.HalfRushAttack(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.lastRushDirection);
                yield return NagaWarrior.RushAttack(NagaWarrior.attackdelayTime / 2);
                NagaWarrior.SpawnAttackCollider(NagaWarrior.lastRushDirection);
                //SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }

            /*protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }*/
        }
        public class AuraAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (NagaWarrior.DistanceToPlayer >= NagaWarrior.MeleeAttackRange) ? 10000 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                NagaWarrior.SpawnSkillCollider(NagaWarrior.DirectionToPlayer);
                yield return new WaitForSeconds(NagaWarrior.attackdelayTime);
                //SetWeights();
                NagaWarrior.ChangeState(NextStateWeights);
            }

            /*protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }*/
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
        /*public new class BackStep : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer <= Elf.RangedAttackRange / 2) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("isMove", true);
                Elf.Animator.SetFloat("moveType", 1);
                yield return Elf.BackStep(Elf.RangedAttackRange);
                Elf.Animator.SetBool("isMove", false);
                SetWeights();
                Elf.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class RushAndAttack : NagaWarriorState
        {
            public RushAndAttack()
            {
                cooldownTime = 10f;
            }
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 2);
                Elf.SkillIndicator.DrawIndicator(SkllIndicatorDrawer.IndicatorType.Line, Elf.SkillIndicator.transform.position, SceneContext.Character.transform, Elf.attackdelayTime * 1.5f, Elf.attackdelayTime / 2);
                yield return new WaitForSeconds(Elf.attackdelayTime);
                Sound.Play("EFFECT_Charge_Complete");
                yield return new WaitForSeconds(Elf.attackdelayTime * 0.5f);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isMove", true);
                Elf.Animator.SetFloat("moveType", 1);
                yield return Elf.RushAttack(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 2);
                Elf.Animator.SetBool("isMove", false);
                Elf.SpawnAttackCollider(Elf.lastRushDirection);
                Elf.Animator.SetBool("isAttack", false);
                yield return new WaitForSeconds(Elf.attackdelayTime);
                SetWeights();
                Elf.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(MeleeAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(WhipAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) && (Elf.CurrentPhase == 1)  ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0},
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.MeleeAttackRange)  ? 50 : 0 }
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class RangedAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 0);
                yield return new WaitForSeconds(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 0);
                Instantiate(Elf.arrowPrefab, Elf.transform.position, Quaternion.identity);
                yield return Elf.arrowSoundObject.Play();
                yield return new WaitForSeconds(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("isAttack", false);
                SetWeights();
                Elf.ChangeState(NextStateWeights);
            }
        }
        public class SeedRangedAttak : NagaWarriorState
        {
            public SeedRangedAttak()
            {
                cooldownTime = 30f;
            }
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer >= Elf.MeleeAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 1);
                Sound.Play("ENEMY_Arrow_Drow");
                yield return new WaitForSeconds(Elf.attackdelayTime);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 1);
                int count = 0;
                while (count < 4)
                {
                    GameObject seed = Instantiate(Elf.seedPrefab, Elf.transform.position, Quaternion.identity);
                    yield return Elf.arrowSoundObject.Play();
                    count++;
                }
                Elf.Animator.SetBool("isAttack", false);
                SetWeights();
                Elf.ChangeState(NextStateWeights);
            }
        }
        public class MeleeAttack : NagaWarriorState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 2);
                yield return new WaitForSeconds(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 2);
                Elf.SpawnAttackCollider(Elf.DirectionToPlayer);
                yield return new WaitForSeconds(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("isAttack", false);
                SetWeights();
                Elf.ChangeState(NextStateWeights);
            }
        }

        public class WhipAttack : NagaWarriorState
        {
            private SoundObject _soundObject;
            public WhipAttack()
            {
                cooldownTime = 30f;
            }
            public override int GetWeight()
            {
                if (Elf.CurrentPhase == 1)
                {
                    return (Elf.DistanceToPlayer < Elf.MeleeAttackRange) ? 5 : 0;
                }
                return 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 3);
                yield return new WaitForSeconds(Elf.attackdelayTime / 3);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 3);
                _soundObject = Sound.Play("EFFECT_Sword_Swing", true);
                AttackObjectController.Create(
                    Elf.transform.position,
                    Vector3.zero,
                    Elf.vinePrefab,
                    new StaticMovement(
                        Elf.transform.position,
                        Elf.attackdelayTime * 2)
                )
                .StartRoutine();
                yield return new WaitForSeconds(Elf.attackdelayTime * 2);
                Sound.Stop(_soundObject);
                Elf.Animator.SetBool("isAttack", false);
                SetWeights();
                Elf.ChangeState(NextStateWeights);
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                Sound.Stop(_soundObject);
            }
        }
        public class TrunkAttack : NagaWarriorState
        {
            public TrunkAttack()
            {
                cooldownTime = 10f;
            }
            public override int GetWeight()
            {
                return (Elf.CurrentPhase == 1) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 4);
                yield return new WaitForSeconds(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 4);
                Sound.Play("EFFECT_Sword_Swing");
                int numberOfObjects = 5;
                float interval = 0.2f;
                float fixedDistance = 7f;

                Vector3 direction = Elf.DirectionToPlayer;
                Vector3 spawnStoneRadius = 2 * direction;
                Vector3 startPosition = Elf.transform.position + spawnStoneRadius;
                yield return new WaitForSeconds(Elf.attackdelayTime);
                Elf.StartCoroutine(SpawnTrunk(startPosition, direction, fixedDistance, numberOfObjects, interval));
                yield return new WaitForSeconds(Elf.attackdelayTime * 2);
                Elf.Animator.SetBool("isAttack", false);
                SetWeights();
                Elf.ChangeState(NextStateWeights);
            }
            private IEnumerator SpawnTrunk(Vector3 startPosition, Vector3 direction, float fixedDistance, int numberOfObjects, float interval)
            {
                for (int i = 0; i <= numberOfObjects; i++)
                {
                    Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
                    AttackObjectController.Create(
                    spawnPosition,
                    Vector3.zero,
                    Elf.trunkPrefab,
                    new StaticMovement(
                        spawnPosition,
                        Elf.attackdelayTime * 2)
                    )
                    .StartRoutine();
                    yield return new WaitForSeconds(interval);
                }
            }
        }*/
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