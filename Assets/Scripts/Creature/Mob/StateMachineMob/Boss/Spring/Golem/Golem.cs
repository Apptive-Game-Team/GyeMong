using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Sound;
using Creature.Attack.Component.Movement;
using Creature.Attack;
using Creature.Boss.Component.SkillIndicator;
using Creature.Boss.Spring.Golem;
using Creature.Mob.Boss.Spring.Elf;
using playerCharacter;
using Unity.VisualScripting;
using UnityEngine;
using System.Drawing;

namespace Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    public class Golem : Boss
    {
        [SerializeField] public GameObject cubePrefab;
        [SerializeField] private GameObject floorPrefab;
        [SerializeField] private GameObject shockwavePrefab;
        [SerializeField] private GameObject pushOutAttackPrefab;
        private Shield shieldComponenet;
        float attackdelayTime = 1f;
        [SerializeField] private SoundObject _shockwavesoundObject;
        public SoundObject ShockwaveSoundObject => _shockwavesoundObject;
        [SerializeField] private SoundObject _tossSoundObject;
        public SoundObject TossSoundObject => _tossSoundObject;
        protected override void Initialize()
        {
            maxPhase = 2;
            maxHps.Clear();
            maxHps.Add(100f);
            maxHps.Add(200f);
            currentHp = maxHps[currentPhase];
            currentShield = 0f;
            damage = 30f;
            currentShield = 0f;
            detectionRange = 10f;
            MeleeAttackRange = 4f;
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
            float targetRadius = 14;
            int startRadius = 4;
            float excludeAngle = Random.Range(-30f, -150f);
            float excludeMin = excludeAngle - 10f;
            float excludeMax = excludeAngle + 10f;

            for (int i = startRadius; i <= targetRadius; i++)
            {
                Vector3[] points = GetCirclePoints(transform.position, i, i * 3 + 10);
                ShockwaveSoundObject.SetSoundSourceByName("ENEMY_Shockwave");
                StartCoroutine(ShockwaveSoundObject.Play());
                foreach (Vector3 point in points)
                {
                    Vector3 dir = (point - transform.position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    if (angle >= excludeMin && angle <= excludeMax)
                        continue;
                    Instantiate(shockwavePrefab, point, Quaternion.identity);
                }
                yield return new WaitForSeconds(attackdelayTime / 3);
            }
        }
        public IEnumerator MakeShock()
        {
            int targetRadius = 4;
            Vector3[] points = GetCirclePoints(transform.position, targetRadius, targetRadius * 3 + 10);
            ShockwaveSoundObject.SetSoundSourceByName("ENEMY_Shockwave");
            StartCoroutine(ShockwaveSoundObject.Play());
            foreach (Vector3 point in points)
            {
                Instantiate(shockwavePrefab, point, Quaternion.identity);
            }
            yield return new WaitForSeconds(attackdelayTime / 3);
        }
        public abstract class GolemState : CoolDownState
        {
            public Golem Golem => mob as Golem;
            protected Dictionary<System.Type, int> weights;
            protected virtual void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                {
                    { typeof(MeleeAttack), (Golem.DistanceToPlayer <= Golem.MeleeAttackRange) ? 500 : 0 },
                    { typeof(FallingCubeAttack), 5 },
                    { typeof(ChargeShield), 50 },
                    { typeof(UpStoneAttack), (Golem.DistanceToPlayer >= Golem.MeleeAttackRange) ? 5 : 0 },
                    { typeof(ShockwaveAttack), (Golem.CurrentPhase == 1) ? 500 : 0 },
                    { typeof(PushOutAttack), (Golem.DistanceToPlayer <= Golem.MeleeAttackRange) ? 5 : 0 }
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

        public class MeleeAttack : GolemState
        {
            public override int GetWeight()
            {
                return (Golem.DistanceToPlayer < Golem.MeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("TwoHand", true);
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);
                yield return Golem.MakeShock();
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);
                Golem.Animator.SetBool("TwoHand", false);
                Golem.ChangeState(NextStateWeights);
            }
        }
        public class PushOutAttack : GolemState
        {
            public override int GetWeight()
            {
                return (Golem.DistanceToPlayer < Golem.MeleeAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("Toss", true);
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);
                Instantiate(Golem.pushOutAttackPrefab, PlayerCharacter.Instance.transform.position - Golem.DirectionToPlayer * 0.5f, Quaternion.identity);
                yield return new WaitForSeconds(Golem.attackdelayTime / 2);
                Golem.Animator.SetBool("Toss", false);
                Golem.ChangeState(NextStateWeights);
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
                GameObject cube = Instantiate(Golem.cubePrefab, PlayerCharacter.Instance.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
                Golem.Animator.SetBool("Toss", false);
                yield return new WaitUntil(() => cube.IsDestroyed());
                Golem.ChangeState(NextStateWeights);
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
                yield return new WaitForSeconds(2f);
                Golem.currentShield = 30f;
                Golem.MaterialController.SetMaterial(MaterialController.MaterialType.SHIELD);
                Golem.MaterialController.SetFloat(1);

                Golem.ChangeState(NextStateWeights);
            }
        }

        public class UpStoneAttack : GolemState
        {
            public override int GetWeight()
            {
                return (Golem.CurrentPhase == 0) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Golem.Animator.SetBool("OneHand", true);
                yield return new WaitForSeconds(Golem.attackdelayTime);

                int numberOfObjects = 5;
                float interval = 0.2f;
                float fixedDistance = 7f;

                List<GameObject> spawnedObjects = new List<GameObject>();

                Vector3 direction = Golem.DirectionToPlayer;
                Vector3 spawnStoneRadius = 2 * direction;
                Vector3 startPosition = Golem.transform.position + spawnStoneRadius;

                Golem.StartCoroutine(SpawnFloor(startPosition, direction, fixedDistance, numberOfObjects, interval, spawnedObjects));

                Golem.Animator.SetBool("OneHand", false);

                yield return new WaitForSeconds(Golem.attackdelayTime * 2);

                Golem.ChangeState(NextStateWeights);
            }

            private IEnumerator SpawnFloor(Vector3 startPosition, Vector3 direction, float fixedDistance, int numberOfObjects, float interval, List<GameObject> spawnedObjects)
            {
                for (int i = 0; i <= numberOfObjects; i++)
                {
                    Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
                    GameObject floor = Instantiate(Golem.floorPrefab, spawnPosition, Quaternion.identity);
                    spawnedObjects.Add(floor);
                    Golem._shockwavesoundObject.SetSoundSourceByName("ENEMY_Shockwave");
                    Golem.StartCoroutine(Golem._shockwavesoundObject.Play());
                    yield return new WaitForSeconds(interval);
                }
                yield return spawnedObjects;
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

                yield return new WaitForSeconds(Golem.attackdelayTime);
                yield return Golem.MakeShockwave();
                yield return new WaitForSeconds(Golem.attackdelayTime / 3);
                Golem.Animator.SetBool("TwoHand", false);
                Golem.ChangeState(NextStateWeights);
            }
        }
        protected override void Die()
        {
            base.Die();
            RootPatternManger.Instance.DeActivateRootObjects();
        }
    }
}