using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Sound;
using Creature.Boss.Component.SkillIndicator;
using Creature.Mob.StateMachineMob.Boss.Spring.Golem;
using playerCharacter;
using UnityEngine;
using static Creature.Mob.StateMachineMob.Boss.Spring.Golem.Golem;

namespace Creature.Mob.Boss.Spring.Elf
{
    public class Elf : StateMachineMob.Boss.Boss
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

        protected override void Initialize()
        {
            maxPhase = 2;
            maxHps.Clear();
            maxHps.Add(100f);
            maxHps.Add(200f);
            currentHp = maxHps[currentPhase];
            damage = 20f;
            speed = 2f;
            currentShield = 0f;
            detectionRange = 10f;
            MeleeAttackRange = 2f;
            RangedAttackRange = 8f;
            SkillIndicator = transform.Find("SkillIndicator").GetComponent<SkllIndicatorDrawer>();
        }
        public abstract class ElfState : CoolDownState
        {
            public Elf Elf => mob as Elf;
            protected Dictionary<System.Type, int> weights;
            public override void OnStateUpdate()
            {
                Elf.Animator.SetFloat("xDir", Elf.DirectionToPlayer.x);
                Elf.Animator.SetFloat("yDir", Elf.DirectionToPlayer.y);
            }
            protected virtual void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(BackStep), (Elf.DistanceToPlayer <= Elf.RangedAttackRange / 2) ? 5 : 0 },
                        { typeof(RushAndAttack), (Elf.DistanceToPlayer >= Elf.RangedAttackRange / 2) ? 50 : 0 },
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.RangedAttackRange / 2) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.RangedAttackRange / 2)  ? 50 : 0 },
                        { typeof(MeleeAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) ? 5 : 0},
                        { typeof(WhipAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) && (Elf.CurrentPhase == 1) ? 50 : 0 },
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
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
                    SetWeights();
                    return weights;
                }
            }
        }
        public new class BackStep : ElfState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer < Elf.RangedAttackRange / 2) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("isMove", true);
                Elf.Animator.SetFloat("moveType", 1);
                yield return Elf.BackStep(Elf.RangedAttackRange);
                Elf.Animator.SetBool("isMove", false);
                Elf.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.RangedAttackRange / 2) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.RangedAttackRange / 2) ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0}
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class RushAndAttack : ElfState
        {
            public RushAndAttack()
            {
                cooldownTime = 10f;
            }
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 2);
                Elf.SkillIndicator.DrawIndicator(SkllIndicatorDrawer.IndicatorType.Line, Elf.SkillIndicator.transform.position, PlayerCharacter.Instance.transform, Elf.attackdelayTime * 1.5f, Elf.attackdelayTime / 2);
                yield return new WaitForSeconds(Elf.attackdelayTime * 1.5f);
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
                Elf.ChangeState(NextStateWeights);
            }
            protected override void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        { typeof(MeleeAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) ? 5 : 0 },
                        { typeof(WhipAttack), (Elf.DistanceToPlayer <= Elf.MeleeAttackRange) && (Elf.CurrentPhase == 1)  ? 50 : 0},
                        { typeof(TrunkAttack), (Elf.CurrentPhase == 1) ? 3 : 0},
                        { typeof(RangedAttack), (Elf.DistanceToPlayer >= Elf.RangedAttackRange / 2) ? 5 : 0 },
                        { typeof(SeedRangedAttak), (Elf.DistanceToPlayer >= Elf.RangedAttackRange / 2)  ? 50 : 0 }
                    };
                if (weights.Values.All(w => w == 0))
                {
                    weights[typeof(MeleeAttack)] = 1;
                }
            }
        }
        public class RangedAttack : ElfState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0;
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
                Elf.ChangeState(NextStateWeights);
            }
        }
        public class SeedRangedAttak : ElfState
        {
            public SeedRangedAttak()
            {
                cooldownTime = 30f;
            }
            public override int GetWeight()
            {
                if (Elf.CurrentPhase == 1)
                {
                    return (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0;
                }
                return 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 1);
                yield return new WaitForSeconds(Elf.attackdelayTime);
                //���� ���� �ʿ�
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
                Elf.ChangeState(NextStateWeights);
            }
        }
        public class MeleeAttack : ElfState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer < Elf.MeleeAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 2);
                yield return new WaitForSeconds(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 2);
                //���� ���� �ʿ�
                Elf.SpawnAttackCollider(Elf.DirectionToPlayer);
                yield return new WaitForSeconds(Elf.attackdelayTime / 2);
                Elf.Animator.SetBool("isAttack", false);
                Elf.ChangeState(NextStateWeights);
            }
        }

        public class WhipAttack : ElfState
        {
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
                //���� ���� �ʿ�
                GameObject vine = Instantiate(Elf.vinePrefab, Elf.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(Elf.attackdelayTime * 2);
                Elf.Animator.SetBool("isAttack", false);
                Elf.ChangeState(NextStateWeights);
            }
        }
        public class TrunkAttack : ElfState
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
                int numberOfObjects = 5;
                float interval = 0.2f;
                float fixedDistance = 7f;

                List<GameObject> spawnedObjects = new List<GameObject>();

                Vector3 direction = Elf.DirectionToPlayer;
                Vector3 spawnStoneRadius = 2 * direction;
                Vector3 startPosition = Elf.transform.position + spawnStoneRadius;
                //�ִϸ��̼�, ���� ���� �ʿ�
                yield return new WaitForSeconds(Elf.attackdelayTime);
                Elf.StartCoroutine(SpawnTrunk(startPosition, direction, fixedDistance, numberOfObjects, interval, spawnedObjects));
                yield return new WaitForSeconds(Elf.attackdelayTime * 2);
                Elf.Animator.SetBool("isAttack", false);
                Elf.ChangeState(NextStateWeights);
            }
            private IEnumerator SpawnTrunk(Vector3 startPosition, Vector3 direction, float fixedDistance, int numberOfObjects, float interval, List<GameObject> spawnedObjects)
            {
                for (int i = 0; i <= numberOfObjects; i++)
                {
                    Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
                    GameObject floor = Instantiate(Elf.trunkPrefab, spawnPosition, Quaternion.identity);
                    spawnedObjects.Add(floor);
                    yield return new WaitForSeconds(interval);
                }
                yield return spawnedObjects;
            }
        }
        protected override void TransPhase()
        {
            base.TransPhase();
        }
        protected override void Die()
        {
            base.Die();
            Animator.SetBool("isDown", true);
        }
        private void SpawnAttackCollider(Vector3 direction)
        {
            Vector2 spawnPosition = transform.position + direction * 1f;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);
            GameObject attackCollider = Instantiate(meleeAttackPrefab, spawnPosition, spawnRotation, transform);
            Destroy(attackCollider, attackdelayTime / 2);
        }
        public override IEnumerator Stun(float duration) //이후 Boss로 올리기
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