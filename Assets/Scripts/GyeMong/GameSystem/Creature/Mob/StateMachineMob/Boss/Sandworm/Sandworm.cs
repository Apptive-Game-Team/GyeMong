using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GyeMong.EventSystem.Event.Boss;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.Material;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.SoundSystem;
using Sequence = DG.Tweening.Sequence;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Sandworm
{
    public class Sandworm : Boss
    {
        [Header("Pattern Prefabs")]
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

        [Header("Movement")] 
        [SerializeField] private SandwormMovement movement;

        [Header("Head Attack Move Value")] 
        [SerializeField] private float headAttackMovePreDelay;
        [SerializeField] private float headAttackMoveDuration;
        [SerializeField] private float headAttackMovePostDelay;
        [SerializeField] private float headAttackMoveBackDelay;
        
        [Header("Attack Move Value")]
        [SerializeField] private float attackMovePreAngle;
        [SerializeField] private float attackMovePreDelay;
        [SerializeField] private float attackMoveAngle;
        [SerializeField] private float attackMoveDuration;
        [SerializeField] private float attackMovePostDelay;
        
        [Header("Laser Attack Move Value")]
        [SerializeField] private float laserAttackMovePreAngle; 
        [SerializeField] private float laserAttackMovePreDelay;
        [SerializeField] private float laserAttackMoveAngle;
        [SerializeField] private float laserAttackMoveDuration;
        [SerializeField] private float laserAttackMovePostDelay;

        [Header("Body Attack Move Value")] 
        [SerializeField] private float bodyAttackSpeed;
        
        [Header("Sound")]
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
            
            movement.FacePlayer();
            ChangeState();
        }

        public abstract class SandwormState : CoolDownState
        {
            protected Sandworm Sandworm => mob as Sandworm;
            protected Dictionary<System.Type, int> _weights;

            public override void OnStateUpdate()
            {
                Sandworm.movement.FacePlayer();
            }

            protected virtual void SetWeights()
            {
                _weights = new Dictionary<System.Type, int>
                    {
                        {typeof(VenomBreath), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        {typeof(HeadAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange) ? 5 : 0 },
                        {typeof(FlameLaser), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) ? 5 : 0 },
                        // {typeof(ShortBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange)
                        //     && (Sandworm.currentPhase == 1) ? 5 : 0 },
                        // {typeof(LongBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange)
                        //     && (Sandworm.currentPhase == 1) ? 5 : 0 },
                        {typeof(ShortBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) && 
                                                      (Sandworm.DistanceToPlayer > Sandworm.MeleeAttackRange) ? 5 : 0 },
                        {typeof(LongBurstOutAttack), (Sandworm.DistanceToPlayer < Sandworm.RangedAttackRange) &&
                                                     (Sandworm.DistanceToPlayer > Sandworm.MeleeAttackRange) ? 50 : 0 },
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
                Sandworm.movement.isIdle = false;
                bool startPositionFlag = Sandworm.DirectionToPlayer.x > 0;
                Vector3 attackPosition = SceneContext.Character.transform.position;
                Sound.Play("ENEMY_Venom_Breath_Action");
                Sandworm.StartCoroutine(Sandworm.movement.AttackMove
                    (startPositionFlag, Sandworm.attackMovePreAngle, Sandworm.attackMovePreDelay,
                        Sandworm.attackMoveAngle, Sandworm.attackMoveDuration, Sandworm.attackMovePostDelay));
                yield return new WaitForSeconds(Sandworm.attackMovePreDelay + Sandworm.attackMoveDuration - 0.1f);
                Sandworm.VenomBreathAttack(attackPosition);
                yield return new WaitForSeconds(Sandworm.attackMovePostDelay + 0.2f);
                Sandworm.movement.isIdle = true;
                yield return new WaitForSeconds(Sandworm.attackMovePostDelay / 2);
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
                Sandworm.movement.isIdle = false;
                Vector3 attackPosition = SceneContext.Character.transform.position;
                attackPosition.y -= 0.4f;
                Sound.Play("ENEMY_Ground_Crash_Action");
                Sandworm.StartCoroutine(Sandworm.movement.HeadAttackMove
                    (attackPosition, Sandworm.headAttackMoveDuration, Sandworm.headAttackMovePreDelay, Sandworm.headAttackMovePostDelay, Sandworm.headAttackMoveBackDelay));
                GameObject indicator = Instantiate(Sandworm.groundCrashIndicator, attackPosition + new Vector3(0.055f, 0.057f, 0f),
                    Quaternion.Euler(0f, 0f, 90f));
                Destroy(indicator, Sandworm.headAttackMovePostDelay + Sandworm.headAttackMoveDuration);
                yield return new WaitForSeconds(Sandworm.headAttackMovePostDelay + Sandworm.headAttackMoveDuration);
                Sound.Play("ENEMY_Ground_Crash");
                GameObject crash = Instantiate(Sandworm.groundCrash, attackPosition, Quaternion.identity);
                Destroy(crash, 0.7f);
                yield return new WaitForSeconds(Sandworm.headAttackMoveBackDelay + 0.4f);
                Sandworm.movement.isIdle = true;
                yield return new WaitForSeconds(Sandworm.headAttackMoveBackDelay / 2);
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
                Sandworm.movement.isIdle = false;
                bool startPositionFlag = Sandworm.DirectionToPlayer.x > 0;
                Sound.Play("ENEMY_Laser_Action");
                Sandworm.StartCoroutine(Sandworm.movement.AttackMove
                    (startPositionFlag, Sandworm.laserAttackMovePreAngle, Sandworm.laserAttackMovePreDelay
                        , Sandworm.laserAttackMoveAngle, Sandworm.laserAttackMoveDuration, Sandworm.laserAttackMovePostDelay));
                Vector3 attackPosition = SceneContext.Character.transform.position;
                yield return new WaitForSeconds(Sandworm.laserAttackMovePreDelay + Sandworm.laserAttackMoveDuration - 0.1f);
                Sandworm.LaserAttack(attackPosition);
                yield return new WaitForSeconds(Sandworm.laserAttackMovePostDelay + 0.2f);
                Sandworm.movement.isIdle = true;
                yield return new WaitForSeconds(Sandworm.laserAttackMovePostDelay / 2);
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
                Sandworm.movement.isIdle = false;
                Sandworm.mapPattern.StopPattern();
                yield return Sandworm.HideOrShow(true, 1f);
                Sandworm.StartCoroutine(Sandworm.ChasePlayer(2f, Sandworm._chaseSpeed * 1.3f));
                yield return new WaitForSeconds(2f);
                Sound.Play("ENEMY_Short_Burst_Action");
                GameObject body = Instantiate(Sandworm.bodyAttack, Sandworm.transform.position, Quaternion.identity);
                Destroy(body, 0.07f);
                yield return Sandworm.HideOrShow(false, 0.3f);
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
                    {typeof(HeadAttack), (Sandworm.DistanceToPlayer < Sandworm.MeleeAttackRange) ? 1 : 0},
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
                Sandworm.movement.isIdle = false;
                Sandworm.mapPattern.StopPattern();
                yield return Sandworm.HideOrShow(true, 1f);
                Sandworm.StartCoroutine(Sandworm.ChasePlayer(2f, Sandworm._chaseSpeed * 0.7f));
                yield return new WaitForSeconds(2f);
                Vector3 attackPosition = SceneContext.Character.transform.position + Sandworm.DirectionToPlayer * 2f;
                yield return new WaitForSeconds(0.25f);
                Sandworm.GetComponentInChildren<ParticleSystem>().Stop();
                Sound.Play("ENEMY_Long_Burst_Action");
                yield return Sandworm.movement.BodyAttackMove(attackPosition, Sandworm.bodyAttackSpeed);
                yield return new WaitForSeconds(0.1f);
                Sandworm.GetComponentInChildren<ParticleSystem>().Play();
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
                Sandworm.movement.isIdle = true;
                yield return new WaitForSeconds(0.1f);
                Sandworm.GetComponent<Collider2D>().enabled = false;
                Sound.Play("ENEMY_Sand_Trap_Action");
                Sandworm.movement.isIdle = false;
                GameObject indicator = Instantiate(Sandworm.megaGroundCrashIndicator, Sandworm.transform.position + new Vector3(0.46f, 0.77f, 0f),
                    Quaternion.Euler(0f, 0f, 90f));
                Destroy(indicator, 3.6f);
                //Sandworm.RotateHead(-30f, 3.5f, 30f, 0.2f, 0.5f);
                Sandworm.StartCoroutine(Sandworm.Scream(3f, 0.05f));
                Sandworm.StartCoroutine(Sandworm.PlayerPull(3f, Sandworm._sunctionSpeed));
                yield return new WaitForSeconds(3.6f);
                Sound.Play("ENEMY_Sand_Trap");
                GameObject groundAttack = Instantiate(Sandworm.megaGroundCrash, Sandworm.transform.position, Quaternion.identity);
                Destroy(groundAttack, 1f);
                yield return new WaitForSeconds(1f);
                yield return Sandworm.StartCoroutine(Sandworm.ChangePhaseEvent());
                Sandworm.GetComponent<Collider2D>().enabled = true;
                Sandworm.movement.isIdle = true;
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

        private void VenomBreathAttack(Vector3 attackPosition)
        {
            Vector2[] directions = new Vector2[3];
            Vector3 startPos = movement.sandwormBody[0].transform.position;
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
            float dist = Vector3.Distance(startPos, targetPos);
            Vector3 peakPos = (startPos + targetPos) / 2 + (Vector3)(direction.x > 0
                ? new Vector2(-direction.y, direction.x)
                : new Vector2(direction.y, -direction.x));
                
            
            venom.transform.DOPath(
                new[] { startPos, peakPos, targetPos},
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
        
        private void LaserAttack(Vector3 attackPosition)
        {
            Vector3 start = movement.sandwormBody[0].transform.position;
            
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

        private IEnumerator HideOrShow(bool hide, float duration)
        {
            GetComponent<Collider2D>().enabled = !hide;
            Sound.Play(hide ? "ENEMY_Hide" : "ENEMY_Show");
            yield return movement.HideOrShow(hide, duration);
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
            StartCoroutine(movement.AttackMove(DirectionToPlayer.x > 0,0,0,50f, 1f, 0, 50f));
            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().sortingOrder--;
            StartCoroutine((new HideBossHealthBarEvent() { _boss = this }).Execute());
            StageManager.ClearStage(this);
        }
    }
}
