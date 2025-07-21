using System.Collections;
using DG.Tweening;
using GyeMong.EventSystem.Event;
using GyeMong.EventSystem.Event.Input;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component.Movement;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.pathfinder;
using GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime.Components;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.GameSystem.Creature.Player;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Slime
{
    public class DivisionSlime : SlimeBase
    {
        private const float DIVIDE_RATIO = 0.6f;
        private int _divisionLevel = 0;
        private int _maxDivisionLevel = 2;
        private Tween _dashTween;
        private bool _isTutorial;
        private bool _isTutorialShown;
        private float _jumpHeight;
        private Coroutine _stunCoroutine;
        
        protected override void Start()
        {
            Initialize();
            DivisionSlimeManager.Instance.RegisterSlime(this);
        }

        protected override void Initialize()
        {
            maxHp = 10f;
            currentHp = maxHp;
            speed = 2f;
            
            _detector = SimplePlayerDetector.Create(this);
            _pathFinder = new SimplePathFinder();
            _slimeAnimator = SlimeAnimator.Create(gameObject, sprites);
            
            MeleeAttackRange = 3f;
            RangedAttackRange = 8f;
            detectionRange = 20f;

            damage = 10f;
            _jumpHeight = 1.2f;
        }

        public override void StartMob()
        {
            _faceToPlayerCoroutine = StartCoroutine(FaceToPlayer());
            _isTutorial = !PlayerPrefs.HasKey("TutorialFlag");
            ChangeState();
        }

        public override void OnAttacked(float damage)
        {
            base.OnAttacked(damage);
            if (currentState is not SlimeDieState)
            {
                if (_dashTween != null && _dashTween.IsActive()) _dashTween.Kill();
                if (currentHp > 0)
                {
                    if (_stunCoroutine != null)
                    {
                        StopCoroutine(_stunCoroutine);
                        _stunCoroutine = null;
                    }
                    _stunCoroutine = StartCoroutine(Stun(1f));
                    StartCoroutine(GetComponent<AirborneController>().AirborneTo(transform.position - DirectionToPlayer * MeleeAttackRange));
                }
                _slimeAnimator.Stop();
                _slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle);
                if (currentHp <= 0) ChangeState(new DieState(this));
            }
        }

        public class SlimeRangedAttackState : RangedAttackState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                return DivisionSlime.DistanceToPlayer > DivisionSlime.MeleeAttackRange && DivisionSlime.DistanceToPlayer < DivisionSlime.RangedAttackRange ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                if (DivisionSlime._isTutorial && !DivisionSlime._isTutorialShown) 
                    yield return DivisionSlime.GrazeSystemTutorial1();
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.RangedAttack);
                Sound.Play("ENEMY_DivisionSlime_RangedAttack");
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                AttackObjectController.Create(
                    mob.transform.position, 
                    mob.DirectionToPlayer, 
                    DivisionSlime.rangedAttack,
                    new LinearMovement(
                        mob.transform.position, 
                        mob.transform.position + mob.DirectionToPlayer * mob.RangedAttackRange, 
                        10f)
                ).StartRoutine();
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true);
                yield return new WaitForSeconds(1);
                if (DivisionSlime._isTutorial) 
                    yield return DivisionSlime.GrazeSystemTutorial2();
                mob.ChangeState();
            }
        }

        public class SlimeMeleeAttackState : MeleeAttackState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                if (DivisionSlime._isTutorial) return 0;
                return DivisionSlime.DistanceToPlayer <= DivisionSlime.MeleeAttackRange ? 5 : 0;
            }
            
            public override IEnumerator StateCoroutine()
            {
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MeleeAttack);
                Sound.Play("ENEMY_DivisionSlime_MeleeAttack");
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime * 2);
                if (mob.DistanceToPlayer <= mob.MeleeAttackRange)   
                    SceneContext.Character.TakeDamage(mob.damage);
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true);
                yield return new WaitForSeconds(1);
                mob.ChangeState();
            }
        }
        
        public class DashAttackState : SlimeState
        {
            private DivisionSlime DivisionSlime => mob as DivisionSlime;
            public override int GetWeight()
            {
                if (DivisionSlime._isTutorial) return 0;
                return (DivisionSlime.DistanceToPlayer > DivisionSlime.MeleeAttackRange &&
                        DivisionSlime.DistanceToPlayer < DivisionSlime.RangedAttackRange) ? 5 : 0;
            }

            public override IEnumerator StateCoroutine()
            {
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.MeleeAttack);
                Vector3 currentPos = DivisionSlime.transform.position;
                Vector2 dashDirection = DivisionSlime.DirectionToPlayer;
                Vector3 dashTargetPosition =
                    SceneContext.Character.transform.position + (Vector3)Random.insideUnitCircle;
                float distanceToTarget = Vector3.Distance(currentPos, dashTargetPosition);
                RaycastHit2D hit = Physics2D.Raycast(currentPos, dashDirection, distanceToTarget, LayerMask.GetMask("Wall"));
                if (hit.collider != null)
                {
                    dashTargetPosition = hit.point - dashDirection * 0.05f;
                }
                Sound.Play("ENEMY_DivisionSlime_DashAttack");
                yield return new WaitForSeconds(2 * SlimeAnimator.AnimationDeltaTime);
                float scaleY = DivisionSlime.transform.localScale.y;
                DivisionSlime._dashTween = DivisionSlime.transform.
                    DOJump(dashTargetPosition, DivisionSlime._jumpHeight, 1, 1f).SetEase(Ease.OutQuad).
                    OnComplete(() => { DivisionSlime.transform.DOScaleY(0.7f * scaleY, 0.2f).SetLoops(2, LoopType.Yoyo); });
                yield return DivisionSlime._dashTween.WaitForCompletion();
                yield return new WaitForSeconds(SlimeAnimator.AnimationDeltaTime);
                DivisionSlime._slimeAnimator.AsyncPlay(SlimeAnimator.AnimationType.Idle, true);
                yield return new WaitForSeconds(1);
                DivisionSlime.ChangeState();
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

        public class MoveState : SlimeMoveState { }

        public class SlimeIdleState : IdleState { }

        private void Divide()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 spawnPosition = transform.position;
                int attempts = 10;
                do
                {
                    Vector3 candidatePosition = transform.position + Random.insideUnitSphere;
                    candidatePosition.z = 0f;
                    var hit = Physics2D.Raycast(transform.position, candidatePosition - transform.position, Vector3.Distance(transform.position, candidatePosition), LayerMask.GetMask("Wall"));
                    if (hit.collider == null)
                    {
                        spawnPosition = candidatePosition;
                        break;
                    }
                } while (attempts-- > 0);
                
                GameObject newSlime = Instantiate(gameObject, transform.position, Quaternion.identity);
                newSlime.transform.localScale = transform.localScale * DIVIDE_RATIO;
                
                newSlime.transform.DOJump(spawnPosition, 1f, 1, 0.5f).SetEase(Ease.OutQuad);

                DivisionSlime slimeComponent = newSlime.GetComponent<DivisionSlime>();
                
                slimeComponent._slimeAnimator = SlimeAnimator.Create(slimeComponent.gameObject, sprites);
                slimeComponent._detector = SimplePlayerDetector.Create(slimeComponent);
                slimeComponent.damage *= DIVIDE_RATIO;
                slimeComponent.MeleeAttackRange *= DIVIDE_RATIO;
                slimeComponent.RangedAttackRange *= DIVIDE_RATIO;
                slimeComponent._jumpHeight *= DIVIDE_RATIO;
                slimeComponent._divisionLevel = _divisionLevel + 1;
                slimeComponent._faceToPlayerCoroutine = slimeComponent.StartCoroutine(slimeComponent.FaceToPlayer());
                slimeComponent.ChangeState();
                
                DivisionSlimeManager.Instance.RegisterSlime(slimeComponent);
            }
        }

        private IEnumerator GrazeSystemTutorial1()
        {
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
            yield return new WaitUntil(() =>
                (player.CurrentHp < player.stat.HealthMax || player.CurrentSkillGauge > 0 || Time.time > time + 1f));
            if (player.CurrentSkillGauge > 0)
            {
                yield return new WaitForSeconds(0.5f);
                yield return StartCoroutine((new SetKeyInputEvent() { _isEnable = false }).Execute());
                yield return StartCoroutine((new SkippablePopupWindowEvent()
                    { Title = "스치기 시스템 배우기", Message = "좌상단에 늘어난 게이지를 이용해 특수공격(마우스 우클릭)을 사용할 수 있다.", Duration = 3f }).Execute());
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
    }
}