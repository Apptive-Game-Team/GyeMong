using System.Collections;
using System.Collections.Generic;
using Creature.Player.Component.Collider;
using playerCharacter;
using TMPro;
using UnityEngine;
using static Creature.Minion.Slime.Slime;
namespace Creature.Boss.Spring.Elf
{
    public class Elf : Boss
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject seedPrefab;
        [SerializeField] private GameObject vinePrefab;
        [SerializeField] private GameObject trunkPrefab;
        [SerializeField] private GameObject meleeAttackPrefab;
        //[SerializeField] private GameObject BombPrefab;
        Vector3 meleeAttackPrefabPos;

        private FootSoundController footSoundController;
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
            footSoundController = transform.Find("FootSoundObject").GetComponent<FootSoundController>();
        }
        public abstract class ElfState : BossState
        {
            public Elf Elf => creature as Elf;
            public override void OnStateUpdate()
            {
                Elf.Animator.SetFloat("xDir", Elf.DirectionToPlayer.x);
                Elf.Animator.SetFloat("yDir", Elf.DirectionToPlayer.y);
            }
            public override Dictionary<System.Type, int> GetNextStateWeights()
            {
                return new Dictionary<System.Type, int>
                {
                    { typeof(BackStep), (Elf.DistanceToPlayer < Elf.RangedAttackRange / 2) ? 5 : 0 },
                    { typeof(RushAttack), (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0 },
                    { typeof(RangedAttack), (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0 },
                    { typeof(SeedRangedAttak), (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) && (Elf.CurrentPhase == 1)  ? 5 : 0 },
                    { typeof(MeleeAttack), (Elf.DistanceToPlayer < Elf.MeleeAttackRange) ? 5 : 0},
                    { typeof(WhipAttack), (Elf.DistanceToPlayer < Elf.MeleeAttackRange) && (Elf.CurrentPhase == 1) ? 5 : 0 },
                    { typeof(TrunkAttack), 3}
                };
            }
        }

        /*public class MoveState : ElfState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer > Elf.MeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("isMove", true);
                Elf.Animator.SetFloat("moveType", 0);
                float duration = 2f;
                float timer = 0f;

                while (duration > timer && Elf.DistanceToPlayer > Elf.MeleeAttackRange)
                {
                    timer += Time.deltaTime;
                    yield return null;
                    Elf.TrackPlayer();
                }

                Elf.Animator.SetBool("isMove", false);
                Elf.ChangeState();
            }
        }*/

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
                Elf.ChangeState();
            }
            public override Dictionary<System.Type, int> GetNextStateWeights()
            {
                return new Dictionary<System.Type, int>
                {
                    { typeof(RangedAttackState), (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0 },
                    { typeof(SeedRangedAttak), (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) && (Elf.CurrentPhase == 1)  ? 5 : 0},
                    { typeof(TrunkAttack),  3}
                };
            }
        }
        public new class RushAttack : ElfState
        {
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer > Elf.RangedAttackRange / 2) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("isMove", true);
                Elf.Animator.SetFloat("moveType", 1);
                yield return Elf.RushAttack();

                Elf.Animator.SetBool("isMove", false);
                Elf.ChangeState();
            }
            public override Dictionary<System.Type, int> GetNextStateWeights()
            {
                return new Dictionary<System.Type, int>
                {
                    { typeof(MeleeAttack), (Elf.DistanceToPlayer < Elf.MeleeAttackRange) ? 5 : 0 },
                    { typeof(WhipAttack), (Elf.DistanceToPlayer < Elf.MeleeAttackRange) && (Elf.CurrentPhase == 1)  ? 5 : 0},
                    { typeof(TrunkAttack),  3}
                };
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
                yield return new WaitForSeconds(0.5f);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 0);
                GameObject arrow = Instantiate(Elf.arrowPrefab, Elf.transform.position, Quaternion.identity);
                yield return Elf.arrowSoundObject.Play();
                yield return new WaitForSeconds(1f);
                Elf.Animator.SetBool("isAttack", false);
                Elf.ChangeState();
            }
        }
        public class SeedRangedAttak : ElfState
        {
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
                yield return new WaitForSeconds(1f);
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
                Elf.ChangeState();
            }
        }
        public class MeleeAttack : ElfState
        {
            float meleeAttackdelayTime = 0.5f;
            public override int GetWeight()
            {
                return (Elf.DistanceToPlayer < Elf.MeleeAttackRange) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                Elf.Animator.SetBool("attackDelay", true);
                Elf.Animator.SetFloat("attackType", 2);
                yield return new WaitForSeconds(meleeAttackdelayTime);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 2);
                SpawnAttackCollider();
                yield return new WaitForSeconds(meleeAttackdelayTime);
                Elf.Animator.SetBool("isAttack", false);
                Elf.ChangeState();
            }
            private void SpawnAttackCollider()
            {
                Vector3 direction = Elf.DirectionToPlayer;
                Vector2 spawnPosition = Elf.transform.position + direction * 1f;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

                GameObject attackCollider = Instantiate(Elf.meleeAttackPrefab, spawnPosition, spawnRotation, Elf.transform);
                Destroy(attackCollider, meleeAttackdelayTime);
            }
        }
        
        public class WhipAttack : ElfState
        {
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
                yield return new WaitForSeconds(0.2f);
                Elf.Animator.SetBool("attackDelay", false);
                Elf.Animator.SetBool("isAttack", true);
                Elf.Animator.SetFloat("attackType", 3);
                GameObject vine = Instantiate(Elf.vinePrefab, Elf.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(2f);
                Elf.Animator.SetBool("isAttack", false);
                Elf.ChangeState();
            }
        }
        public class TrunkAttack : ElfState
        {
            public override int GetWeight()
            {
                return (Elf.CurrentPhase == 0) ? 5 : 0;
            }
            public override IEnumerator StateCoroutine()
            {
                int numberOfObjects = 5;
                float interval = 0.2f;
                float fixedDistance = 7f;

                List<GameObject> spawnedObjects = new List<GameObject>();

                Vector3 direction = Elf.DirectionToPlayer;
                Vector3 spawnStoneRadius = 2 * direction;
                Vector3 startPosition = Elf.transform.position + spawnStoneRadius;

                yield return new WaitForSeconds(1f);
                Elf.StartCoroutine(SpawnTrunk(startPosition, direction, fixedDistance, numberOfObjects, interval, spawnedObjects));
                yield return new WaitForSeconds(2f);
                Elf.ChangeState();
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
        /*public class TransPhasePattern : ElfState
        {
            public float warningDuration = 2f;
            public int numWarnings = 8;
            public float range = 10f;
            public override int GetWeight()
            {
                return 0;
            }
            public override IEnumerator StateCoroutine()
            {
                List<Vector2> warningPositions = GenerateWarning(numWarnings, range);
                foreach (var position in warningPositions)
                {
                    Instantiate(Elf.BombPrefab, position, Quaternion.identity);
                }
                yield return new WaitForSeconds(warningDuration);
                Elf.ChangeState();
            }
            private List<Vector2> GenerateWarning(int count, float range)
            {
                List<Vector2> positions = new List<Vector2>();

                for (int i = 0; i < count; i++)
                {
                    float x = Elf.transform.position.x + Random.Range(-range, range);
                    float y = Elf.transform.position.y + Random.Range(-range, range);
                    Vector2 randomPosition = new Vector2(x, y);
                    positions.Add(randomPosition);
                }
                return positions;
            }
        }*/
        protected override void TransPhase()
        {
            base.TransPhase(); 
        }
        protected override void Die()
        {
            base.Die();
            Animator.SetBool("isDown", true);
        }
    }
}