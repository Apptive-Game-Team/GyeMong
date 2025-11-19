using System;
using System.Collections;
using DG.Tweening;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.pathfinder;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.GameSystem.Creature.Player.Component.Collider;
using GyeMong.GameSystem.Indicator;
using GyeMong.SoundSystem;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime
{
    public enum SlimeType
    {
        Idle,
        Melee,
        Ranged,
    }
    
    public class DivisionSlime : SlimeBase
    {
        [SerializeField] private GameObject slimeSprite;
        [SerializeField] private GameObject bounceAttackPrefab;
        private SlimeHpBar _hpBar;
        private CapsuleCollider2D _bounceAttackCollider;
        private GameObject _playerCollider;
        private SlimeType _type = SlimeType.Idle;
        private const float DivideRatio = 0.6f;
        private const int DivideNumber = 2;
        private int _divisionLevel = 0;
        private int _maxDivisionLevel = 2;
        private Tween _dashTween;
        private Tween _jumpTween;
        private float _scale;
        private bool _isTutorial;
        private bool _isTutorialShown;

        public Action OnHpChanged;
        
        protected override void Start()
        {
            Initialize();
            DivisionSlimeManager.Instance.RegisterSlime(this);
            _bounceAttackCollider = bounceAttackPrefab.GetComponent<CapsuleCollider2D>();
            _playerCollider = SceneContext.Character.GetComponentInChildren<HitCollider>().gameObject;
            _hpBar = FindAnyObjectByType<SlimeHpBar>(FindObjectsInactive.Include);
        }

        protected override void Initialize()
        {
            maxHp = 10f;
            currentHp = maxHp;
            speed = 2f;
            
            _detector = SimplePlayerDetector.Create(this);
            _pathFinder = new SimplePathFinder();
            _slimeAnimator = SlimeAnimator.Create(slimeSprite, sprites);
            
            RangedAttackRange = 10f;
            detectionRange = 20f;
        }

        public override void StartMob()
        {
            _faceToPlayerCoroutine = StartCoroutine(FaceToPlayer());
            _isTutorial = !PlayerPrefs.HasKey("TutorialFlag");
            _scale = gameObject.transform.localScale.x;
            ChangeState();
        }

        public override void OnAttacked(float damage)
        {
            _hpBar.currentHp -= Mathf.Min(currentHp, damage);
            OnHpChanged?.Invoke();
            base.OnAttacked(damage);
            if (currentState is not SlimeDieState)
            {
                if (currentHp <= 0)
                {
                    if (_dashTween != null && _dashTween.IsActive()) _dashTween.Kill();
                    ChangeState(new DieState(this));
                }
            }
        }

        private void SetRangedAttack(Vector3 shootPos, Vector3 direction, Vector3 target)
        {
            AttackObjectController rangedAttackObject = AttackObjectController.Create(
                shootPos,
                direction,
                rangedAttack,
                new ParabolicMovement(
                    shootPos,
                    target,
                    12f * Vector3.Distance(transform.position, target) / RangedAttackRange)
            );
            rangedAttackObject.transform.localScale = transform.localScale * 3 / 4;
            rangedAttackObject.StartRoutine();
        }

        private int SpriteDirection(Vector3 dir)
        {
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0)
            {
                angle += 360f;
            }
            
            int sector = Mathf.FloorToInt(angle / 60f);
            return sector;
        }

        public class SlimeRangedAttackState : RangedAttackState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                if (DivisionSlime._type == SlimeType.Melee) return 0;
                return !DivisionSlime.IsInBounceAttackRange() && 
                       DivisionSlime.DistanceToPlayer < DivisionSlime.RangedAttackRange * 1.5f ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                if (DivisionSlime._isTutorial && !DivisionSlime._isTutorialShown)
                {
                    yield return DivisionSlime.GrazeSystemTutorial1();
                }
                DivisionSlime.StopCoroutine(DivisionSlime._faceToPlayerCoroutine);
                Vector3 offset = new Vector3(0, 0.4f, 0);
                Vector3 scaledOffset = Vector3.Scale(offset, DivisionSlime.transform.lossyScale);
                Vector3 shootPos = DivisionSlime.transform.position + scaledOffset;
                Vector3 targetPos = SceneContext.Character.transform.position;
                Vector3 direction = (targetPos - shootPos).normalized;
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.RangedAttack, false, DivisionSlime.SpriteDirection(DivisionSlime.DirectionToPlayer));
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 2);
                Sound.Play("ENEMY_DivisionSlime_RangedAttack");
                DivisionSlime.SetRangedAttack(shootPos, direction, targetPos);
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 3);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true, DivisionSlime.SpriteDirection(DivisionSlime.DirectionToPlayer));
                DivisionSlime._faceToPlayerCoroutine = DivisionSlime.StartCoroutine(DivisionSlime.FaceToPlayer());
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                if (DivisionSlime._isTutorial)
                {
                    yield return DivisionSlime.GrazeSystemTutorial2();
                }
                mob.ChangeState();
            }
        }

        public class SlimeBounceAttackState : SlimeState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                if (DivisionSlime._isTutorial) return 0;
                return DivisionSlime.IsInBounceAttackRange() ? 5 : 0;
            }
            
            public override IEnumerator StateCoroutine()
            {
                DivisionSlime.StopCoroutine(DivisionSlime._faceToPlayerCoroutine);
                Vector3 scale = DivisionSlime.transform.localScale;
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MeleeAttack, false, DivisionSlime.SpriteDirection(DivisionSlime.DirectionToPlayer));
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 2);
                GameObject bounceAttack = Instantiate(DivisionSlime.bounceAttackPrefab, DivisionSlime.transform.position - new Vector3(0, 0.7f, 0) * DivisionSlime.transform.localScale.x, Quaternion.identity);
                float scaleValue = DivisionSlime.transform.localScale.x;
                bounceAttack.transform.localScale *= scaleValue;
                
                ParticleSystem particle = bounceAttack.GetComponentInChildren<ParticleSystem>();
                particle.gameObject.transform.localScale *= scaleValue;
                
                // Emission Count
                var emission = particle.emission;
                ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
                emission.GetBursts(bursts);
                bursts[0].minCount = (short)(bursts[0].minCount * scaleValue);
                bursts[0].maxCount = (short)(bursts[0].maxCount * scaleValue);
                emission.SetBursts(bursts);
                
                // Start Size
                var main = particle.main;
                var startSize = main.startSize;
                float originalMin = startSize.constantMin;
                float originalMax = startSize.constantMax;
                float newMin = originalMin * scaleValue;
                float newMax = originalMax * scaleValue;
                main.startSize = new ParticleSystem.MinMaxCurve(newMin, newMax);

                yield return IndicatorGenerator.Instance.GenerateIndicator(bounceAttack,
                    DivisionSlime.transform.position - new Vector3(0, 0.7f, 0) * DivisionSlime.transform.localScale.x, Quaternion.identity, SlimeAnimator.AnimationDeltaTime * 2);
                Sound.Play("ENEMY_DivisionSlime_MeleeAttack");
                bounceAttack.SetActive(true);
                particle.Play();
                Destroy(bounceAttack, SlimeAnimator.AnimationDeltaTime);
                
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                DivisionSlime._faceToPlayerCoroutine = DivisionSlime.StartCoroutine(DivisionSlime.FaceToPlayer());
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                mob.ChangeState();
            }
        }
        
        public class DashAttackState : SlimeState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                if (DivisionSlime._isTutorial) return 0;
                if (DivisionSlime._type == SlimeType.Ranged) return 0;
                return (!DivisionSlime.IsInBounceAttackRange() &&
                        DivisionSlime.DistanceToPlayer < DivisionSlime.RangedAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                DivisionSlime.StopCoroutine(DivisionSlime._faceToPlayerCoroutine);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, false, DivisionSlime.SpriteDirection(DivisionSlime.DirectionToPlayer));
                Vector3 currentPos = DivisionSlime.transform.position;
                Vector2 dashDirection = DivisionSlime.DirectionToPlayer;
                Vector3 dashTargetPosition = SceneContext.Character.transform.position + (Vector3)dashDirection * DivisionSlime._scale / 4;
                float distanceToTarget = Vector3.Distance(currentPos, dashTargetPosition);
                RaycastHit2D hit = Physics2D.Raycast(currentPos, dashDirection, distanceToTarget, LayerMask.GetMask("Wall"));
                if (hit.collider != null)
                {
                    dashTargetPosition = hit.point - dashDirection * 0.05f;
                }
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                Vector3 middlePos = (currentPos + dashTargetPosition * 3) / 4;
                float jumpHeight = Vector3.Distance(DivisionSlime.transform.position, middlePos) / 4;
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.DashAttack, false, DivisionSlime.SpriteDirection(DivisionSlime.DirectionToPlayer));
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 2);
                Sound.Play("ENEMY_DivisionSlime_DashAttack");
                Sequence seq1 = DOTween.Sequence();
                DivisionSlime._dashTween = DivisionSlime.transform.DOMove(middlePos, 2 * SlimeAnimator.AnimationDeltaTime).SetEase(Ease.OutQuad);
                DivisionSlime._jumpTween = DivisionSlime.slimeSprite.transform
                    .DOJump(middlePos, jumpHeight, 1,
                        2 * SlimeAnimator.AnimationDeltaTime).SetEase(Ease.OutQuad);
                seq1.Append(DivisionSlime._dashTween);
                seq1.Join(DivisionSlime._jumpTween);
                yield return seq1.WaitForCompletion();
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 4);
                if (DivisionSlime.IsInBounceAttackRange())
                {
                    DivisionSlime._faceToPlayerCoroutine = DivisionSlime.StartCoroutine(DivisionSlime.FaceToPlayer());
                    DivisionSlime.ChangeState();
                }
                else
                {
                    DivisionSlime._faceToPlayerCoroutine = DivisionSlime.StartCoroutine(DivisionSlime.FaceToPlayer());
                    yield return new WaitForSeconds(0.1f);
                    DivisionSlime.StopCoroutine(DivisionSlime._faceToPlayerCoroutine);
                    dashTargetPosition = SceneContext.Character.transform.position + (Vector3)dashDirection * DivisionSlime._scale / 4;
                    DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.DashAttack, false, DivisionSlime.SpriteDirection(DivisionSlime.DirectionToPlayer));
                    yield return new WaitForSeconds(2 * SlimeAnimator.AnimationDeltaTime);
                    jumpHeight = Vector3.Distance(DivisionSlime.transform.position, dashTargetPosition) / 4;
                    Sound.Play("ENEMY_DivisionSlime_DashAttack");
                    Sequence seq2 = DOTween.Sequence();
                    DivisionSlime._dashTween = DivisionSlime.transform
                        .DOMove(dashTargetPosition, 2 * SlimeAnimator.AnimationDeltaTime).SetEase(Ease.OutQuad);
                    DivisionSlime._jumpTween = DivisionSlime.slimeSprite.transform
                        .DOJump(dashTargetPosition, jumpHeight, 1, 2 * SlimeAnimator.AnimationDeltaTime).SetEase(Ease.OutQuad);
                    seq2.Append(DivisionSlime._dashTween);
                    seq2.Join(DivisionSlime._jumpTween);
                    yield return seq2.WaitForCompletion();
                    yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 4);
                    DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, false, DivisionSlime.SpriteDirection(DivisionSlime.DirectionToPlayer));
                    DivisionSlime._faceToPlayerCoroutine = DivisionSlime.StartCoroutine(DivisionSlime.FaceToPlayer());
                    DivisionSlime.ChangeState();
                }
            }
        }

        public class DieState : SlimeDieState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public DieState() { }
            public DieState(StateMachineMob mob)
            {
                this.mob = mob;
            }
            public override int GetWeight()
            {
                return 0;
            }
            public override IEnumerator StateCoroutine()
            {
                DivisionSlime._slimeAnimator.Stop();
                if (DivisionSlime._divisionLevel < DivisionSlime._maxDivisionLevel)
                {
                    DivisionSlime.Divide();
                }
                else
                {
                    DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Die);
                    yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                }
                DivisionSlimeManager.Instance.UnregisterSlime(DivisionSlime);
                Destroy(DivisionSlime.gameObject);
            }
        }

        protected override void OnDead()
        {
            
        }

        public class MoveState : SlimeMoveState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                if (DivisionSlime._type == SlimeType.Ranged && DivisionSlime.DistanceToPlayer < DivisionSlime.RangedAttackRange) return 0;
                return !DivisionSlime.IsInBounceAttackRange() && DivisionSlime.DistanceToPlayer < DivisionSlime.DetectionRange ? 5 : 0;
            }
        }

        public class SlimeIdleState : IdleState { }

        private void Divide()
        {
            gameObject.transform.localScale = new Vector3(_scale, _scale, _scale);
            for (int i = 0; i < DivideNumber; i++)
            {
                Vector3 spawnPosition = transform.position;
                int attempts = 10;
                do
                {
                    Vector3 candidatePosition = transform.position + Random.insideUnitSphere * 2;
                    candidatePosition.z = 0f;
                    var hit = Physics2D.Raycast(transform.position, (candidatePosition - transform.position).normalized,
                        Vector3.Distance(transform.position, candidatePosition), LayerMask.GetMask("Wall"));
                    if (hit.collider == null)
                    {
                        spawnPosition = candidatePosition;
                        break;
                    }
                } while (attempts-- > 0);
                
                GameObject newSlime = Instantiate(gameObject, transform.position, Quaternion.identity);
                newSlime.transform.localScale = transform.localScale * DivideRatio;
                
                newSlime.transform.DOJump(spawnPosition, 1f, 1, 0.5f).SetEase(Ease.OutQuad);

                DivisionSlime slimeComponent = newSlime.GetComponent<DivisionSlime>();
                slimeComponent._hpBar = _hpBar;
                
                slimeComponent._slimeAnimator = SlimeAnimator.Create(slimeComponent.gameObject, sprites);
                slimeComponent._detector = SimplePlayerDetector.Create(slimeComponent);
                slimeComponent.RangedAttackRange = RangedAttackRange * DivideRatio;
                slimeComponent._scale = _scale * DivideRatio;
                slimeComponent._divisionLevel = _divisionLevel + 1;
                slimeComponent._type = (SlimeType)(i + 1);
                slimeComponent._bounceAttackCollider = bounceAttackPrefab.GetComponent<CapsuleCollider2D>();
                slimeComponent._playerCollider = SceneContext.Character.GetComponentInChildren<HitCollider>().gameObject;
                slimeComponent._hpBar.BindAction(slimeComponent);
                slimeComponent._faceToPlayerCoroutine = slimeComponent.StartCoroutine(slimeComponent.FaceToPlayer());
                slimeComponent.ChangeState();
                
                DivisionSlimeManager.Instance.RegisterSlime(slimeComponent);
            }
        }

        private IEnumerator GrazeSystemTutorial1()
        {
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
            yield return StartCoroutine((new SkippablePopupWindowEvent()
                { Title = "스치기 시스템 배우기", Message = "슬라임의 원거리 공격을 아슬아슬하게 피해보자", Duration = 3f }).Execute());
            _isTutorialShown = true;
            yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
        }
        
        private IEnumerator GrazeSystemTutorial2()
        {
            PlayerCharacter player = SceneContext.Character;
            float time = Time.time;
            yield return new WaitUntil(() => player.CurrentSkillGauge > 0 || Time.time > time + 1f);
            if (player.CurrentSkillGauge > 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    {
                        Title = "스치기 시스템 배우기", Message = "좌상단에 늘어난 게이지를 이용해 특수공격(마우스 우클릭)을 사용할 수 있다.", Duration = 3f
                    })
                    .Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "스치기 시스템 배우기", Message = "게이지는 기본 공격을 적중시켜도 미세하게 오른다.", Duration = 3f }).Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "슬라임 잡기", Message = "이제 귀여운 슬라임을 잡아보자!", Duration = 3f }).Execute());
                yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = true }).Execute());
                _isTutorial = false;
                SceneContext.Character.isTutorial = false;
                PlayerPrefs.SetInt("TutorialFlag", 1);
                PlayerPrefs.Save();
            }
        }

        private bool IsInBounceAttackRange()
        {
            float scaleFactor = transform.localScale.x;

            Vector2 scaledSize = _bounceAttackCollider.size * scaleFactor;
            Vector2 scaledOffset = _bounceAttackCollider.offset * scaleFactor;
            Vector2 center = (Vector2)transform.position + scaledOffset - new Vector2(0, 0.7f);
            float angle = transform.eulerAngles.z;
            
            Collider2D[] hit = Physics2D.OverlapCapsuleAll(
                point: center,
                size: scaledSize,
                direction: _bounceAttackCollider.direction,
                angle: angle,
                layerMask: LayerMask.GetMask("Player")
            );

            bool flag = false;
            foreach (var col in hit)
            {
                if (col.gameObject == _playerCollider) flag = true;
            }

            return flag;
        }

        public float GetTotalHp()
        {
            float curHp = maxHp;
            float hp = curHp;
            int count = DivideNumber;
            for (int i = 0; i < _maxDivisionLevel; i++)
            {
                hp += curHp * count;
                count *= DivideNumber;
            }

            return hp;
        }
    }
}