using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class Sandworm : Boss
    {
        [SerializeField] private GameObject venomAttack;
        private float _venomDistance;
        private float _venomDuration;
        private float _venomSpreadAngle;
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
            RangedAttackRange = 100f;

            _venomDistance = 8f;
            _venomDuration = 0.8f;
            _venomSpreadAngle = 15f;

            ChangeState(new VenomBreath(){mob = this});
        }

        public abstract class SandwormState : CoolDownState
        {
            public Sandworm Sandworm => mob as Sandworm;
            protected Dictionary<System.Type, int> weights;
            protected bool IsActionExist = false;

            public override void OnStateUpdate()
            {
                if (!IsActionExist)
                {
                    Sandworm.GetComponent<SpriteRenderer>().flipX = Sandworm.DirectionToPlayer.x > 0;
                }
            }

            protected virtual void SetWeights()
            {
                weights = new Dictionary<System.Type, int>
                    {
                        {typeof(VenomBreath), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 100 : 0 },
                        {typeof(HeadAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(FlameLaser), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(ShortBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(LongBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(SandTrapAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 }
                    };
            }
            
            protected Dictionary<System.Type, int> NextStateWeights
            {
                get
                {   
                    return weights;
                }
            }
        }

        public class VenomBreath : SandwormState
        {
            public override int GetWeight()
            {
                return 1;
            }

            public override IEnumerator StateCoroutine()
            {
                IsActionExist = true;
                Sandworm.RotateHead(-15f, 0.5f, 20f, 0.2f, 0.4f);
                yield return new WaitForSeconds(0.5f);
                Sandworm.VenomBreathAttack();
                yield return new WaitForSeconds(0.6f);
                IsActionExist = false;
                yield return new WaitForSeconds(0.4f);
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
            }
        }

        public class HeadAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class FlameLaser : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class ShortBurstOutAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class LongBurstOutAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        public class SandTrapAttack : SandwormState
        {
            public override int GetWeight()
            {
                return 0;
            }

            public override IEnumerator StateCoroutine()
            {
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
        }

        private void RotateHead(float upAngle, float upTime, float downAngle, float downTime, float recoverTime)
        {
            Sequence spitSequence = DOTween.Sequence();
            if (DirectionToPlayer.x > 0)
            {
                upAngle = -upAngle;
                downAngle = -downAngle;
            }
            
            spitSequence.Append(transform.DORotate(new Vector3(0, 0, upAngle), upTime)
                .SetEase(Ease.OutSine));
            spitSequence.Append(transform.DORotate(new Vector3(0, 0, downAngle), downTime)
                .SetEase(Ease.InQuad));
            spitSequence.Append(transform.DORotate(Vector3.zero, recoverTime)
                .SetEase(Ease.OutBack));
        }

        private void VenomBreathAttack()
        {
            // 3방향 회전 벡터
            Vector2[] directions = new Vector2[3];
            directions[0] = DirectionToPlayer;
            directions[1] = RotateVector(DirectionToPlayer, _venomSpreadAngle);
            directions[2] = RotateVector(DirectionToPlayer, -_venomSpreadAngle);

            foreach (var dir in directions)
            {
                SpawnPoisonProjectile(dir);
            }
        }

        private void SpawnPoisonProjectile(Vector2 direction)
        {
            // 타겟 위치 계산 (지면)
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + (Vector3)(direction.normalized * _venomDistance);

            // Instantiate
            GameObject projectile = Instantiate(venomAttack, startPos, Quaternion.identity);

            // 포물선 효과용 y축 중간점 추가
            Vector3 peakPos = Vector3.Lerp(startPos, targetPos, 0.5f) + Vector3.up * 1.5f;

            // DoTween으로 곡선 이동 (경로는 start → peak → end)
            projectile.transform.DOPath(
                new Vector3[] { startPos, peakPos, targetPos },
                _venomDuration,
                PathType.CatmullRom
            ).SetEase(Ease.InOutSine);
            // .OnComplete(() =>
            // {
            //     // 착지 시 장판 생성
            //     Instantiate(poisonAreaPrefab, targetPos, Quaternion.identity);
            // });
        }

        // 방향 벡터 회전 함수 (2D 기준)
        private Vector2 RotateVector(Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            return new Vector2(
                v.x * cos - v.y * sin,
                v.x * sin + v.y * cos
            );
        }
    }
}
