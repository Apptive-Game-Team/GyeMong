using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GyeMong.GameSystem.Creature.Player;
using Unity.VisualScripting;
using Sequence = DG.Tweening.Sequence;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class Sandworm : Boss
    {
        [SerializeField] private GameObject venomAttack;
        [SerializeField] private GameObject venomPit;
        [SerializeField] private GameObject groundCrash;
        private float _venomAttackDistance;
        private float _venomAttackDuration;
        private float _venomAttackSpreadAngle;
        private float _venomPitDuration;
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
            MeleeAttackRange = 3f;
            RangedAttackRange = 100f;

            _venomAttackDistance = 8f;
            _venomAttackDuration = 0.8f;
            _venomAttackSpreadAngle = 15f;
            _venomPitDuration = 2f;

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
                        {typeof(HeadAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange) ? 100 : 0 },
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
                IsActionExist = true;
                Vector3 attackPosition = PlayerCharacter.Instance.transform.position;
                attackPosition.y -= 0.4f; 
                Sandworm.RotateHead(-20f, 1f, 50f, 0.2f, 0.4f);
                yield return new WaitForSeconds(0.9f);
                GameObject crash = Instantiate(Sandworm.groundCrash, attackPosition, Quaternion.identity);
                Destroy(crash, 0.7f);
                yield return new WaitForSeconds(0.6f);
                IsActionExist = false;
                yield return new WaitForSeconds(0.4f);
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
            Vector2[] directions = new Vector2[3];
            directions[0] = DirectionToPlayer;
            directions[1] = RotateVector(DirectionToPlayer, _venomAttackSpreadAngle);
            directions[2] = RotateVector(DirectionToPlayer, -_venomAttackSpreadAngle);

            foreach (var dir in directions)
            {
                SpawnVenomAttack(dir);
            }
        }

        private void SpawnVenomAttack(Vector2 direction)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + (Vector3)(direction.normalized * _venomAttackDistance);
            
            GameObject venom = Instantiate(venomAttack, startPos, Quaternion.identity);
            
            Vector3 peakPos = Vector3.Lerp(startPos, targetPos, 0.5f) + Vector3.up * 1.5f;
            
            venom.transform.DOPath(
                new[] { startPos, peakPos, targetPos },
                _venomAttackDuration,
                PathType.CatmullRom
            ).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                if (!venom || !venom.activeInHierarchy) return;
                Destroy(venom);
                GameObject pit = Instantiate(venomPit, targetPos, Quaternion.identity);
                Destroy(pit, _venomPitDuration);
            });
        }
        
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
