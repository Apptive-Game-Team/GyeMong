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
        [SerializeField] private GameObject laserAttack;
        [SerializeField] private GameObject bodyAttack;
        private float _venomAttackDuration;
        private float _venomAttackSpreadAngle;
        private float _venomPitDuration;
        private float _laserDuration;
        private float _laserDistance;
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
            MeleeAttackRange = 5f;
            RangedAttackRange = 100f;
            
            _venomAttackDuration = 0.8f;
            _venomAttackSpreadAngle = 15f;
            _venomPitDuration = 2f;
            _laserDuration = 1f;
            _laserDistance = 4f;

            ChangeState();
        }

        public abstract class SandwormState : CoolDownState
        {
            protected Sandworm Sandworm => mob as Sandworm;
            protected Dictionary<System.Type, int> _weights;
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
                _weights = new Dictionary<System.Type, int>
                    {
                        {typeof(VenomBreath), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 1 : 0 },
                        {typeof(HeadAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange) ? 1 : 0 },
                        {typeof(FlameLaser), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 1 : 0 },
                        {typeof(ShortBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange) ? 5 : 0 },
                        {typeof(LongBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange) ? 20 : 0 },
                        {typeof(SandTrapAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 }
                    };
            }
            
            protected Dictionary<System.Type, int> NextStateWeights
            {
                get
                {   
                    return _weights;
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
                bool startPositionFlag = Sandworm.DirectionToPlayer.x > 0;
                Vector3 attackPosition = PlayerCharacter.Instance.transform.position;
                Sandworm.RotateHead(-15f, 0.5f, 20f, 0.2f, 0.4f);
                yield return new WaitForSeconds(0.6f);
                Sandworm.VenomBreathAttack(startPositionFlag, attackPosition);
                yield return new WaitForSeconds(0.5f);
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
                IsActionExist = true;
                bool startPositionFlag = Sandworm.DirectionToPlayer.x > 0;
                Sandworm.RotateHead(-20f, 0.2f, 0f, 0.2f, 0f);
                Vector3 attackPosition = PlayerCharacter.Instance.transform.position;
                yield return new WaitForSeconds(0.3f);
                Sandworm.LaserAttack(startPositionFlag, attackPosition);
                yield return new WaitForSeconds(1f);
                IsActionExist = false;
                yield return new WaitForSeconds(0.4f);
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
                IsActionExist = true;
                Sandworm.HideOrShow(true, 0.3f);
                yield return new WaitForSeconds(0.3f);
                Sandworm.StartCoroutine(Sandworm.ChasePlayer(2f));
                yield return new WaitForSeconds(2f);
                Sandworm.HideOrShow(false, 0.3f);
                GameObject body = Instantiate(Sandworm.bodyAttack, Sandworm.transform.position, Quaternion.identity);
                Destroy(body, 0.07f);
                IsActionExist = false;
                yield return new WaitForSeconds(0.4f);
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
                IsActionExist = true;
                Sandworm.HideOrShow(true, 0.3f);
                yield return new WaitForSeconds(0.3f);
                Sandworm.StartCoroutine(Sandworm.ChasePlayer(2f));
                yield return new WaitForSeconds(2f);
                Vector3 attackPosition = PlayerCharacter.Instance.transform.position;
                yield return new WaitForSeconds(0.5f);
                Sandworm.GetComponentInChildren<ParticleSystem>().Stop();
                Sandworm.HideOrShow(false, 0.1f);
                Sandworm.JumpToPlayer(attackPosition, 0.7f);
                yield return new WaitForSeconds(0.7f);
                Sandworm.GetComponentInChildren<ParticleSystem>().Play();
                Sandworm.HideOrShow(true, 0.2f);
                yield return new WaitForSeconds(0.2f);
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }

            protected override void SetWeights()
            {
                _weights = new Dictionary<System.Type, int>
                {
                    {typeof(ShortBurstOutAttack), 1}
                };
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

        private void VenomBreathAttack(bool startPositionFlag, Vector3 attackPosition)
        {
            Vector2[] directions = new Vector2[3];
            directions[0] = (attackPosition - transform.position).normalized;
            directions[1] = RotateVector(directions[0], _venomAttackSpreadAngle);
            directions[2] = RotateVector(directions[0], -_venomAttackSpreadAngle);

            foreach (var dir in directions)
            {
                SpawnVenomAttack(startPositionFlag, dir, attackPosition);
            }
        }

        private void SpawnVenomAttack(bool startPositionFlag, Vector2 direction, Vector3 attackPosition)
        {
            Vector3 startPos = startPositionFlag
                ? transform.position + new Vector3(2.6f, 1f, 0f)
                : transform.position + new Vector3(-2.6f, 1f, 0f);
            Vector3 targetPos = startPos + (Vector3)direction * (attackPosition - startPos).magnitude + (Vector3)Random.insideUnitCircle;
            
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
        
        private void LaserAttack(bool startPositionFlag,Vector3 attackPosition)
        {
            Vector3 start = startPositionFlag
                ? transform.position + new Vector3(2.6f, 1f, 0f)
                : transform.position + new Vector3(-2.6f, 1f, 0f);
            
            Vector3 dir = (attackPosition - transform.position).normalized;
            Vector3 startPos = attackPosition - dir * _laserDistance;
            Vector3 endPos = attackPosition + dir * _laserDistance;     // 레이저가 땅에 닿는 시작 부분, 끝 부분 지정
            
            Vector3 startDirection = (startPos - start).normalized;
            Vector3 endDirection = (endPos - start).normalized;
            float startLength = (startPos - start).magnitude;
            float endLength = (endPos - start).magnitude; // 레이저의 처음 각도, 끝 각도, 처음 길이, 끝 길이 지정
            
            float startAngle = Mathf.Atan2(startDirection.y, startDirection.x) * Mathf.Rad2Deg;
            float endAngle = Mathf.Atan2(endDirection.y, endDirection.x) * Mathf.Rad2Deg;       // 레이저의 각도를 오일러 각도로 변환
            
            GameObject laser = Instantiate(laserAttack, start, Quaternion.identity);
            StartCoroutine(UpdateLaser(laser.transform, start, startPos, endPos, _laserDuration));
        }
        
        private IEnumerator UpdateLaser(Transform laserTransform, Vector3 fixedStart, Vector3 moveStart, Vector3 moveEnd, float duration)
        {
            float time = 0f;
            float realLength = 3.23f;

            while (time < duration)
            {
                float t = time / duration;
                Vector3 currentTarget = Vector3.Lerp(moveStart, moveEnd, t);
                Vector3 dir = currentTarget - fixedStart;

                float length = dir.magnitude;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                
                laserTransform.position = fixedStart;
                laserTransform.rotation = Quaternion.Euler(0f, 0f, angle);
                laserTransform.localScale = new Vector3(length / realLength, 1f, 1f);

                time += Time.deltaTime;
                yield return null;
            }
            Destroy(laserTransform.gameObject);
        }

        private void HideOrShow(bool hide, float duration)
        {
            float targetX = hide ? -90f : 0f;
            Vector3 newRotation = new Vector3(targetX, 0f, 0f);

            transform.DORotate(newRotation, duration).SetEase(Ease.InOutSine);
        }

        private IEnumerator ChasePlayer(float duration)
        {
            float time = 0f;

            while (time < duration)
            {
                Vector3 target = PlayerCharacter.Instance.transform.position;
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 3);
                time += Time.deltaTime;
                yield return null;
            }
        }

        private void JumpToPlayer(Vector3 playerPosition, float duration)
        {
            Vector3 startPos = transform.position;

            Vector3 dir = (playerPosition - startPos).normalized;
            Vector3 targetPos = playerPosition + dir * 5f;
            float arcHeight = 0.5f;
            
            GameObject jumpAttack = Instantiate(bodyAttack, transform.position, Quaternion.identity, transform);
            Destroy(jumpAttack, duration);
            
            DOTween.To(() => 0f, t =>
            {
                Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
                pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
                transform.position = pos;
            }, 1f, duration).SetEase(Ease.Linear);
            
            transform.DORotate(new Vector3(0, 0, 720f), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine);
        }
    }
}
