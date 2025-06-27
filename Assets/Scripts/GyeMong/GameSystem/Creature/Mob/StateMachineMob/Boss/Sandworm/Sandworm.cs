using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.SoundSystem;
using Sequence = DG.Tweening.Sequence;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class Sandworm : Boss
    {
        [SerializeField] private TailPattern mapPattern;
        [SerializeField] private GameObject venomAttack;
        [SerializeField] private GameObject venomPit;
        [SerializeField] private GameObject groundCrash;
        [SerializeField] private GameObject groundCrashIndicator;
        [SerializeField] private GameObject megaGroundCrash;
        [SerializeField] private GameObject megaGroundCrashIndicator;
        [SerializeField] private GameObject laserAttack;
        [SerializeField] private GameObject bodyAttack;
        private float _venomAttackDuration;
        private float _venomAttackSpreadAngle;
        private float _venomPitDuration;
        private float _laserDuration;
        private float _laserDistance;
        private float _sunctionSpeed;
        private float _chaseSpeed;
        public SoundObject curBGM;
        public SoundObject burrowingSound;
        protected override void Initialize()
        {
            maxPhase = 2;
            maxHps.Clear();
            maxHps.Add(50f);
            maxHps.Add(100f);
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
            _sunctionSpeed = 3f;
            _chaseSpeed = 4f;

            //ChangeState();
        }

        public abstract class SandwormState : CoolDownState
        {
            protected Sandworm Sandworm => mob as Sandworm;
            protected Dictionary<System.Type, int> _weights;
            protected bool IsActionExist;

            public override void OnStateUpdate()
            {
                if (!IsActionExist)
                {
                    Sandworm.transform.localScale =
                        Sandworm.DirectionToPlayer.x > 0 ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1);
                }
            }

            protected virtual void SetWeights()
            {
                _weights = new Dictionary<System.Type, int>
                    {
                        {typeof(VenomBreath), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(HeadAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange) ? 5 : 0 },
                        {typeof(FlameLaser), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(ShortBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange)
                            && (Sandworm.currentPhase == 1) ? 5 : 0 },
                        {typeof(LongBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange)
                            && (Sandworm.currentPhase == 1) ? 5 : 0 },
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
                return (Sandworm.currentPhase == 0) ? 1 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                IsActionExist = true;
                bool startPositionFlag = Sandworm.DirectionToPlayer.x > 0;
                Vector3 attackPosition = SceneContext.Character.transform.position;
                Sound.Play("ENEMY_Venom_Breath_Action");
                Sandworm.RotateHead(-15f, 0.75f, 20f, 0.2f, 0.4f);
                yield return new WaitForSeconds(0.85f);
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
                Vector3 attackPosition = SceneContext.Character.transform.position;
                attackPosition.y -= 0.4f;
                Sound.Play("ENEMY_Ground_Crash_Action");
                Sandworm.RotateHead(-20f, 1f, 50f, 0.2f, 0.4f);
                GameObject indicator = Instantiate(Sandworm.groundCrashIndicator, attackPosition + new Vector3(0.055f, 0.057f, 0f),
                    Quaternion.Euler(0f, 0f, 90f));
                Destroy(indicator, 0.9f);
                yield return new WaitForSeconds(0.9f);
                Sound.Play("ENEMY_Ground_Crash");
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
                Sound.Play("ENEMY_Laser_Action");
                Sandworm.RotateHead(-20f, 0.2f, 0f, 0.2f, 0f);
                Vector3 attackPosition = SceneContext.Character.transform.position;
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
                Sandworm.mapPattern.StopPattern();
                Sandworm.HideOrShow(true, 0.3f);
                yield return new WaitForSeconds(0.3f);
                Sandworm.StartCoroutine(Sandworm.ChasePlayer(2f, Sandworm._chaseSpeed));
                yield return new WaitForSeconds(2f);
                Sandworm.HideOrShow(false, 0.3f);
                Sound.Play("ENEMY_Short_Burst_Action");
                GameObject body = Instantiate(Sandworm.bodyAttack, Sandworm.transform.position, Quaternion.identity);
                Destroy(body, 0.07f);
                IsActionExist = false;
                yield return new WaitForSeconds(0.2f);
                Sandworm.mapPattern.StartPattern();
                yield return new WaitForSeconds(0.2f);
                SetWeights();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
            
            protected override void SetWeights()
            {
                _weights = new Dictionary<System.Type, int>
                {
                    {typeof(VenomBreath), 1},
                    {typeof(HeadAttack), 1},
                    {typeof(FlameLaser), 1},
                };
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
                Sandworm.mapPattern.StopPattern();
                Sandworm.HideOrShow(true, 0.3f);
                yield return new WaitForSeconds(0.3f);
                Sandworm.StartCoroutine(Sandworm.ChasePlayer(2f, Sandworm._chaseSpeed));
                yield return new WaitForSeconds(2f);
                Vector3 attackPosition = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(0.5f);
                Sandworm.GetComponentInChildren<ParticleSystem>().Stop();
                Sandworm.HideOrShow(false, 0.1f);
                Sound.Play("ENEMY_Long_Burst_Action");
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
                return (Sandworm.currentPhase == 1) ? 1 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                Sandworm.mapPattern.StopPattern();
                IsActionExist = false;
                yield return new WaitForSeconds(0.1f);
                Sandworm.GetComponent<Collider2D>().enabled = false;
                Sound.Play("ENEMY_Sand_Trap_Action");
                IsActionExist = true;
                GameObject indicator = Instantiate(Sandworm.megaGroundCrashIndicator, Sandworm.transform.position + new Vector3(0.46f, 0.77f, 0f),
                    Quaternion.Euler(0f, 0f, 90f));
                Destroy(indicator, 3.6f);
                Sandworm.RotateHead(-30f, 3.5f, 30f, 0.2f, 0.5f);
                Sandworm.StartCoroutine(Sandworm.Scream(3f, 0.05f));
                Sandworm.StartCoroutine(Sandworm.PlayerPull(3f, Sandworm._sunctionSpeed));
                yield return new WaitForSeconds(3.6f);
                Sound.Play("ENEMY_Sand_Trap");
                GameObject groundAttack = Instantiate(Sandworm.megaGroundCrash, Sandworm.transform.position, Quaternion.identity);
                Destroy(groundAttack, 1f);
                yield return new WaitForSeconds(1f);
                yield return Sandworm.StartCoroutine(Sandworm.ChangePhaseEvent());
                Sandworm.GetComponent<Collider2D>().enabled = true;
                IsActionExist = false;
                SetWeights();
                Sandworm.mapPattern.StartPattern();
                Sandworm.ChangeState(NextStateWeights);
                yield return null;
            }
            
            protected override void SetWeights()
            {
                _weights = new Dictionary<System.Type, int>
                {
                    {typeof(LongBurstOutAttack), 1}
                };
            }
        }

        private void RotateHead(float upAngle, float upTime, float downAngle, float downTime, float recoverTime, float recoverAngle = 0f)
        {
            Sequence spitSequence = DOTween.Sequence();
            if (DirectionToPlayer.x > 0)
            {
                upAngle = -upAngle;
                downAngle = -downAngle;
                recoverAngle = -recoverAngle;
            }
            
            spitSequence.Append(transform.DORotate(new Vector3(0, 0, upAngle), upTime)
                .SetEase(Ease.OutSine));
            spitSequence.Append(transform.DORotate(new Vector3(0, 0, downAngle), downTime)
                .SetEase(Ease.InQuad));
            spitSequence.Append(transform.DORotate(new Vector3(0, 0, recoverAngle), recoverTime)
                .SetEase(Ease.OutBack));
        }

        private void VenomBreathAttack(bool startPositionFlag, Vector3 attackPosition)
        {
            Vector2[] directions = new Vector2[3];
            Vector3 startPos = startPositionFlag
                ? transform.position + new Vector3(2.6f, 1f, 0f)
                : transform.position + new Vector3(-2.6f, 1f, 0f);
            directions[0] = (attackPosition - startPos).normalized;
            directions[1] = RotateVector(directions[0], _venomAttackSpreadAngle);
            directions[2] = RotateVector(directions[0], -_venomAttackSpreadAngle);
            Sound.Play("ENEMY_Venom_Breath");
            foreach (var dir in directions)
            {
                SpawnVenomAttack(startPos, dir, attackPosition);
            }
        }

        private void SpawnVenomAttack(Vector3 startPos, Vector2 direction, Vector3 attackPosition)
        {
            Vector3 targetPos = startPos + (Vector3)direction * (attackPosition - startPos).magnitude + (Vector3)Random.insideUnitCircle / 4;
            
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
            
            GameObject laser = Instantiate(laserAttack, start, Quaternion.identity);
            Sound.Play("ENEMY_Laser");
            StartCoroutine(UpdateLaser(laser.transform, start, startPos, endPos, _laserDuration));
        }
        
        private IEnumerator UpdateLaser(Transform laserTransform, Vector3 fixedStart, Vector3 moveStart, Vector3 moveEnd, float duration)
        {
            float time = 0f;
            float realLength = 3.23f;
            Destroy(laserTransform.gameObject, duration);
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
        }

        private void HideOrShow(bool hide, float duration)
        {
            float targetX = hide ? -90f : 0f;
            Vector3 newRotation = new Vector3(targetX, 0f, 0f);
            GetComponent<Collider2D>().enabled = !hide;

            Sound.Play(hide ? "ENEMY_Hide" : "ENEMY_Show");
            transform.DORotate(newRotation, duration).SetEase(Ease.InOutSine);
        }

        private IEnumerator ChasePlayer(float duration, float chaseSpeed)
        {
            burrowingSound = Sound.Play("ENEMY_Burrowing", true);
            float time = 0f;
            while (time < duration)
            {
                Vector3 target = SceneContext.Character.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            Sound.Stop(burrowingSound);
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

        private IEnumerator Scream(float duration, float force)
        {
            Sound.Stop(curBGM);
            Sound.Play("ENEMY_Sand_Trap_Effect");
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                SceneContext.CameraManager.CameraShake(force);
                yield return null;
            }
        }
        
        private IEnumerator PlayerPull(float duration, float force)
        {
            float elapsed = 0f;
            PlayerCharacter player = SceneContext.Character;

            while (elapsed < duration)
            {
                if (!player.isDashing)
                {
                    Vector2 direction = (transform.position - player.transform.position).normalized;
                    player.transform.Translate(direction * (force * Time.deltaTime), Space.World);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        protected override void TransPhase()
        {
            if (currentPhase < maxHps.Count - 1)
            {
                currentPhase++;
                StopAllCoroutines();
                MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
                StartCoroutine(ChangePhase());
            }
            else
            {
                MaterialController.SetMaterial(MaterialController.MaterialType.DEFAULT);
                Die();
            }
        }

        private IEnumerator ChangePhase()
        {
            yield return StartCoroutine((new HideBossHealthBarEvent() { _boss = this }).Execute());
            currentHp = CurrentMaxHp;
            ChangeState();
            yield return null;
        }

        private IEnumerator ChangePhaseEvent()
        {
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = false}).Execute());
            yield return StartCoroutine(SceneContext.EffectManager.FadeOut());
            // change floor to basement
            yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(SceneContext.EffectManager.FadeIn());
            curBGM = Sound.Play("BGM_Summer_Sandworm", true);
            yield return StartCoroutine((new ShowBossHealthBarEvent() { _boss = this }).Execute());
            yield return StartCoroutine( (new SetKeyInputEvent(){_isEnable = true}).Execute());
        }

        protected override void Die()
        {
            currentState.OnStateExit();
            mapPattern.StopPattern();
            StopAllCoroutines();
            Sound.Stop(curBGM);
            Sound.Play("ENEMY_Ground_Crash");
            RotateHead(0,0,50f, 1f, 0, 50f);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().sortingOrder--;
            StartCoroutine((new HideBossHealthBarEvent() { _boss = this }).Execute());
        }
    }
}
