using System.Collections;
using System.Collections.Generic;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Component.Collider;
using GyeMong.GameSystem.Creature.Player.Controller;
using GyeMong.GameSystem.Map.Stage;
using GyeMong.InputSystem;
using UnityEngine;
using DG.Tweening;
using GyeMong.UISystem.Game.BattleUI;

namespace GyeMong.GameSystem.Creature.Player
{
    public class PlayerCharacter : MonoBehaviour, IControllable, IEventTriggerable
    {
        public PlayerChangeListenerCaller changeListenerCaller = new PlayerChangeListenerCaller();
        
        public StatComponent stat;
        [SerializeField] private StatData _statData;
        [SerializeField] private float curHealth;
        [SerializeField] public float curShield;
        [SerializeField] private float curSkillGauge;
        public bool isControlled = false;
        private Vector2 movement;
        private Vector2 lastMovementDirection;
        private Vector2 mousePosition;
        public Vector2 mouseDirection { get; private set; }
        
        private Rigidbody2D playerRb;
        private Animator animator;
        private PlayerSoundController soundController;
        
        public GameObject attackColliderPrefab;
        public GameObject attackComboColliderPrefab;
        public GameObject skillColliderPrefab;
        public GameObject healEffectPrefab;
        public GameObject healCompleteEffectPrefab;
        private GameObject activeHealEffect;
        private GameObject activeHealCompleteEffect;
        private float blinkDelay = 0.2f;

        private bool isMoving = false;     
        private bool canDash = true;
        public bool isDashing = false;
        public bool isAttacking = false;
        private bool isHealing = false;
        private bool canMove = true;
        public bool isInvincible = false;
        private bool canCombo = false;
        private bool comboQueued = false;
        private CircleCollider2D _hitCollider;

        public bool isTutorial;

        private Coroutine healingCoroutine;

        public Material[] materials;
        private Renderer _renderer;

        private Coroutine _attackCoroutine;
        private Tween _attackMoveTween;
        private Tween _dashTween;

        protected void Awake()
        {
            stat = _statData.GetStatComp();
            curHealth = stat.HealthMax;
            curSkillGauge = 0f;
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            soundController = GetComponent<PlayerSoundController>();
            _hitCollider = transform.Find("HitCollider").GetComponent<CircleCollider2D>();
            isTutorial = PlayerPrefs.GetInt("TutorialFlag") == 0;
            _renderer = gameObject.GetComponent<Renderer>();
            _renderer.material = materials[0];
        }

        private void Start()
        {
            changeListenerCaller.CallShieldChangeListeners(curShield);
            changeListenerCaller.CallHpChangeListeners(curHealth);
        }

        private void Update()
        {
            if (!isControlled)
            {
                if (canMove)
                {
                    HandleMoveInput();
                    UpdateState();
                }
                HandleActionInput();
            }
        }

        private void FixedUpdate()
        {
            if (!isControlled)
            {
                MoveCharacter();
            }
        }

        private void HandleMoveInput()
        {
            movement.x = 0;
            movement.y = 0;

            if (InputManager.Instance.GetKey(ActionCode.MoveRight))
            {
                movement.x = 1;
            }
            else if (InputManager.Instance.GetKey(ActionCode.MoveLeft))
            {
                movement.x = -1;
            }

            if (InputManager.Instance.GetKey(ActionCode.MoveUp))
            {
                movement.y = 1;
            }
            else if (InputManager.Instance.GetKey(ActionCode.MoveDown))
            {
                movement.y = -1;
            }

            movement.Normalize();
        }

        private void HandleActionInput()
        {
            if (InputManager.Instance.GetKeyDown(ActionCode.Dash) && canDash)
            {
                StartCoroutine(Dash());
            }

            if ((InputManager.Instance.GetKeyDown(ActionCode.Attack) && curSkillGauge >= stat.AttackCost || InputManager.Instance.GetKey(ActionCode.Attack)) &&
                !isDashing && curSkillGauge >= stat.AttackCost) //test for New Battle System
            {
                if (!isAttacking)
                {
                    _attackCoroutine = StartCoroutine(Attack());
                }
                else if (canCombo)
                {
                    comboQueued = true;
                }
            }

            if (InputManager.Instance.GetKeyDown(ActionCode.Skill) && !isAttacking && curSkillGauge >= stat.SkillCost)
            {
                StartCoroutine(ChargeSkillAttack());
                // StartCoroutine(SkillAttack());
            }

            if (InputManager.Instance.GetKeyDown(ActionCode.Heal) && !isAttacking && curSkillGauge >= stat.HealCost && curHealth != stat.HealthMax)
            {
                healingCoroutine = StartCoroutine(Heal());
            }
        }

        private void MoveCharacter()
        {
            soundController.SetRun(isMoving);
            
            Vector2 currentVelocity = playerRb.velocity;
            currentVelocity = Vector2.Lerp(currentVelocity, movement * stat.MoveSpeed, stat.MoveAcceleration * Time.fixedDeltaTime);

            playerRb.velocity = currentVelocity;
        }

        private void UpdateState()
        {
            isMoving = movement.magnitude > 0;
            animator.SetBool("isMove", isMoving);
            soundController.SetBool(PlayerSoundType.FOOT, isMoving);

            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDirection = (mousePosition - playerRb.position).normalized;

            if (canMove && isMoving)
            {
                lastMovementDirection = movement;
            }

            animator.SetFloat("xDir", mouseDirection.x);
            animator.SetFloat("yDir", mouseDirection.y);
        }

        public void TakeDamage(float damage, bool isUnblockable = false)
        {
            damage = 1;

            if (isInvincible) return;

            if (damage >= curShield && curShield > 0)
            {
                damage -= curShield;
                curShield = 0;
                changeListenerCaller.CallShieldChangeListeners(curShield);
            }
            else if (damage < curShield && curShield > 0)
            {
                curShield -= damage;
                changeListenerCaller.CallShieldChangeListeners(curShield);
            }
            curHealth -= damage;
            PlayerEvent.TriggerOnTakeDamage(damage);
            changeListenerCaller.CallHpChangeListeners(curHealth);
            TakeGauge();

            if (isHealing)
            {
                StopCoroutine(healingCoroutine);
                DestroyEffect(ref activeHealEffect);
                healingCoroutine = null;
                animator.SetBool("isHealing", false);
                isHealing = false;
                isAttacking = false;
                canMove = true;
                Debug.Log("Heal is unfailed");
            }

            if (curHealth <= 0)
            {
                StartCoroutine(TriggerInvincibility());
                Die();
            }
            else
            {
                StartCoroutine(TriggerInvincibility());
            }
        }

        public void TakeGauge()
        {
            curSkillGauge -= stat.GrazeGainOnGraze;
            if (curSkillGauge < 0)
            {
                curSkillGauge = 0f;
            }
            changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);
        }

        public void AttackIncreaseGauge()
        {
            if (isTutorial) return;
            curSkillGauge += stat.GrazeGainOnAttack;
            if (curSkillGauge > stat.GrazeMax)
            {
                curSkillGauge = stat.GrazeMax;
            }
            changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);
        }

        public void GrazeIncreaseGauge(float ratio)
        {
            soundController.Trigger(PlayerSoundType.GRAZE);
            curSkillGauge += stat.GrazeGainOnGraze / ratio;
            if (curSkillGauge > stat.GrazeMax)
            {
                curSkillGauge = stat.GrazeMax;
            }
            changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);
        }
        
        private IEnumerator TriggerInvincibility()
        {
            _renderer.material.SetFloat("_BlinkTrigger", 1f);
            yield return new WaitForSeconds(blinkDelay);
            _renderer.material.SetFloat("_BlinkTrigger", 0f);
            yield return new WaitForSeconds(stat.InvincibilityDuration - blinkDelay);
        }

        private IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            canMove = false;
            movement = Vector2.zero;
            animator.SetBool("isDashing", true);
            soundController.Trigger(PlayerSoundType.DASH);

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
                isAttacking = false;
                animator.SetBool("isAttacking", false);
                animator.SetBool("isAttacking2", false);
            }
            _attackMoveTween?.Kill();

            Vector2 dashDirection = GetCurrentInputDirection(mouseDirection);
            animator.SetFloat("xDir", dashDirection.x);
            animator.SetFloat("yDir", dashDirection.y);
            Vector2 startPosition = playerRb.position;

            RaycastHit2D hit = Physics2D.Raycast(startPosition, dashDirection, stat.DashDistance, LayerMask.GetMask("Wall"));
            Vector2 targetPosition = hit.collider == null
                ? startPosition + dashDirection * stat.DashDistance
                : hit.point + hit.normal * 0.1f;

            SceneContext.Character.changeListenerCaller.CallDashUsed(stat.DashCooldown);

            StartCoroutine(SetInvincibility(stat.DashDuration / 5, stat.DashDuration * 3 / 5));

            yield return (_dashTween = playerRb.DOMove(targetPosition, stat.DashDuration)
                .SetEase(Ease.OutCubic))
                .WaitForCompletion();

            StopPlayer();

            canMove = true;
            isDashing = false;
            animator.SetBool("isDashing", false);

            yield return new WaitForSeconds(stat.DashCooldown);
            canDash = true;
        }

        private IEnumerator SetInvincibility(float delay, float duration)
        {
            yield return new WaitForSeconds(delay);
            isInvincible = true;
            _renderer.material = materials[2];
            _renderer.material.SetFloat("_isUsable", 1f);
            yield return new WaitForSeconds(duration);
            _renderer.material.SetFloat("_isUsable", 0f);
            _renderer.material = materials[0];
            isInvincible = false;
        }

        private Vector2 GetCurrentInputDirection(Vector2 direction)
        {
            var dir = Vector2.zero;
            if (InputManager.Instance.GetKey(ActionCode.MoveUp)) dir += Vector2.up;
            if (InputManager.Instance.GetKey(ActionCode.MoveDown)) dir += Vector2.down;
            if (InputManager.Instance.GetKey(ActionCode.MoveRight)) dir += Vector2.right;
            if (InputManager.Instance.GetKey(ActionCode.MoveLeft)) dir += Vector2.left;

            if (dir != Vector2.zero) return dir.normalized;
            return direction;
        }

        private IEnumerator Attack()
        {
            isAttacking = true;
            canMove = false;
            
            movement = Vector2.zero;
            StopPlayer();
            
            animator.SetBool("isAttacking", true);
            curSkillGauge -= stat.AttackCost; // for test new battle system
            SpawnAttackCollider(attackColliderPrefab);
            soundController.Trigger(PlayerSoundType.SWORD_SWING);
            yield return new WaitForSeconds(stat.AttackDelay / 2);
            AttackMove(GetCurrentInputDirection(mouseDirection));
            
            canCombo = true;
            yield return new WaitForSeconds(stat.AttackDelay); // 콤보 입력 대기
            canCombo = false;

            if (comboQueued)
            {
                comboQueued = false;
                UpdateState();
                soundController.Trigger(PlayerSoundType.SWORD_SWING);
                animator.SetBool("isAttacking2", true);
                curSkillGauge -= stat.AttackCost/2;
                SpawnAttackCollider(attackComboColliderPrefab);
                yield return new WaitForSeconds(stat.AttackDelay / 2);
                AttackMove(GetCurrentInputDirection(mouseDirection));
                yield return new WaitForSeconds(stat.AttackDelay);
                animator.SetBool("isAttacking2", false);
            }
            
            canMove = true;
            animator.SetBool("isAttacking", false);
            isAttacking = false;
            
            yield return null;
        }

        private void AttackMove(Vector2 direction)
        {
            Vector2 currentPos = transform.position;
            Vector2 targetPos = currentPos + direction / 3;
            
            Vector2 dirNormalized = direction.normalized;
            float distance = (direction / 3).magnitude;
            float colliderRadius = _hitCollider.radius;
            float adjustedDistance = distance + colliderRadius;
            RaycastHit2D hit = Physics2D.Raycast(currentPos, dirNormalized, adjustedDistance, LayerMask.GetMask("Wall"));
            if (hit.collider == null && (_dashTween == null || !_dashTween.IsActive() || !_dashTween.IsPlaying())) 
                _attackMoveTween = transform.DOMove(targetPos, stat.AttackDelay).SetEase(Ease.OutCubic);
        }

        private float _chargeThreshold = 1f;
        private float _chargePowerCoef = 1f;
        
        public IEnumerator ChargeSkillAttack()
        {
            float curChargePower = 0f;
            
            soundController.Trigger(PlayerSoundType.SWORD_SWING);
            while (InputManager.Instance.GetKey(ActionCode.Skill) == true && curChargePower < _chargeThreshold)
            {
                curChargePower += Time.deltaTime * _chargePowerCoef;
                yield return null;
            }
            stat.SetStatValue(StatType.SKILL_COEF,StatValueType.FINAL_PERCENT_VALUE, curChargePower);
            yield return SkillAttack();
            stat.SetStatValue(StatType.SKILL_COEF,StatValueType.FINAL_PERCENT_VALUE, 0);
        }
        
        private IEnumerator SkillAttack()
        {
            soundController.Trigger(PlayerSoundType.SWORD_SKILL);
            isAttacking = true;
            canMove = false;
            animator.SetBool("isAttacking", true);

            curSkillGauge -= stat.SkillCost;
            changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);
            SpawnAttackCollider(attackColliderPrefab);
            SpawnSkillCollider();

            movement = Vector2.zero;
            StopPlayer();

            canMove = true;

            yield return new WaitForSeconds(stat.AttackDelay);

            animator.SetBool("isAttacking", false);
            
            isAttacking = false;
        }

        public IEnumerator Heal()
        {
            Debug.Log("IsHealing..");
            animator.SetBool("isHealing", true);

            SpawnHealEffect(healEffectPrefab);

            isHealing = true;
            isAttacking = true;
            canMove = false;
            movement = Vector2.zero;
            StopPlayer();

            float elapsed = 0f;
            float consumedGauge = 0f;

            float tick = 0.1f;
            float costPerTick = stat.HealCost * tick / 1f;

            while (InputManager.Instance.GetKey(ActionCode.Heal))
            {
                if (curSkillGauge < costPerTick)
                {
                    Debug.Log("게이지 부족으로 힐 중단");
                    break;
                }

                curSkillGauge -= costPerTick;
                consumedGauge += costPerTick;
                elapsed += tick;

                changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);

                if (elapsed >= 1f)
                {
                    Heal(stat.HealAmount);
                    SpawnHealCompleteEffect(healCompleteEffectPrefab);
                    soundController.Trigger(PlayerSoundType.HEAL);
                    Debug.Log("Heal is Complete");

                    curSkillGauge -= (stat.HealCost - consumedGauge);
                    break;
                }

                yield return new WaitForSeconds(tick);
            }

            Debug.Log("힐 끝");

            DestroyEffect(ref activeHealEffect);

            animator.SetBool("isHealing", false);
            isHealing = false;
            isAttacking = false;
            canMove = true;
        }

        public void Heal(float amount)
        {
            curHealth += amount;
            if (curHealth > stat.HealthMax)
            {
                curHealth = stat.HealthMax;
            }
            changeListenerCaller.CallHpChangeListeners(curHealth);
        }
        private void SpawnHealEffect(GameObject prefab)
        {
            if (prefab == null) return;
            if (activeHealEffect != null) return;
            DestroyEffect(ref activeHealEffect);
            DestroyEffect(ref activeHealCompleteEffect);
            activeHealEffect = Instantiate(prefab, transform);
            activeHealEffect.transform.localPosition = Vector3.zero;
        }
        private void SpawnHealCompleteEffect(GameObject prefab)
        {
            if (prefab == null) return;
            if (activeHealCompleteEffect != null) return;
            DestroyEffect(ref activeHealEffect);
            DestroyEffect(ref activeHealCompleteEffect);
            activeHealCompleteEffect = Instantiate(prefab, transform);
            activeHealCompleteEffect.transform.localPosition = Vector3.zero;
        }
        private void DestroyEffect(ref GameObject activeEffect)
        {
            if (activeEffect != null)
            {
                Destroy(activeEffect);
                activeEffect = null;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnAttackCollider(GameObject attackPrefab)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDirection = (mousePosition - playerRb.position).normalized;
            Vector2 spawnPosition = playerRb.position + mouseDirection * 0.5f;

            float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject attackCollider = Instantiate(attackPrefab, spawnPosition, spawnRotation, transform);
            attackCollider.GetComponent<AttackCollider>().Init(soundController);
            Destroy(attackCollider, stat.AttackDelay);
        }

        private void SpawnSkillCollider()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDirection = (mousePosition - playerRb.position).normalized;
            Vector2 spawnPosition = playerRb.position + mouseDirection * 0.5f;

            float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject attackCollider = Instantiate(skillColliderPrefab, spawnPosition, spawnRotation, transform);

            Material attackParticle = attackCollider.transform.Find("Particle System").GetComponent<Renderer>().material;
            attackParticle.SetFloat("_Rotation", Mathf.Atan2(mouseDirection.y, mouseDirection.x));

            Rigidbody2D skillRigidbody = attackCollider.GetComponent<Rigidbody2D>();
            skillRigidbody.velocity = mouseDirection.normalized * stat.SkillSpeed;

            AttackCollider atkComp = attackCollider.GetComponent<AttackCollider>();
            atkComp.Init(soundController);
            atkComp.SetDamage(stat.AttackPower * stat.SkillCoef);
            attackCollider.transform.localScale *= stat.SkillCoef;
            Destroy(attackCollider, stat.AttackDelay * 2);
        }

        private void Die()
        {
            //GameOver Event Triggered.
            changeListenerCaller.CallPlayerDied();
            animator.SetBool("isHuck",true);
            GetComponent<AirborneController>()?.StopAllCoroutines();
            StopPlayer();
            Mob.Mob[] mobList = FindObjectsOfType<Mob.Mob>();
            foreach (var mob in mobList)
            {
                StartCoroutine(mob.Stun(float.MaxValue));
            }
            isControlled = true;
        }

        public void SetPlayerMove(bool _canMove)
        {
            canMove = _canMove;
        }

        public void Bind(float duration)
        {
            StartCoroutine(BindCoroutine(duration));
        }

        private IEnumerator BindCoroutine(float duration)
        {
            movement = Vector2.zero;
            playerRb.velocity = Vector2.zero;
            canMove = false;
            yield return new WaitForSeconds(duration);
            canMove = true;
        }

        public Vector3 GetPlayerPosition()
        {
            return gameObject.transform.position;
        }

        public Vector2 GetPlayerDirection()
        {
            return lastMovementDirection;
        }

        public IEnumerator LoadPlayerEffect()
        {
            isControlled = true;

            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material = materials[1];
            float ratio = 0f;
            renderer.material.SetFloat("_Value", ratio);
            yield return new WaitForSeconds(2f);
            while (true)
            {   
                renderer.material.SetFloat("_Value", ratio);
                ratio += 0.04f;
                yield return new WaitForSeconds(0.08f);
                if (ratio > 1.0f) break;
            }
            renderer.material = materials[0];

            isControlled = false;
        }

        public IEnumerator MoveTo(Vector3 target, float speed)
        {
            isControlled = true;
            StopPlayer();
            animator.SetBool("isMove", true);
            soundController.SetBool(PlayerSoundType.FOOT, true);
            
            while (true)
            {
                Vector3 direction = (target - transform.position).normalized;
                
                animator.SetFloat("xDir", direction.x);
                animator.SetFloat("yDir", direction.y);
                
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, step);
                
                if ((target - transform.position).sqrMagnitude <= 0.01f)
                {
                    break;
                }
                animator.SetFloat("speed", speed);
        
                yield return null;
            }
            StopPlayer();
            animator.SetFloat("speed", speed);
            
            animator.SetBool("isMove", false);
            soundController.SetBool(PlayerSoundType.FOOT, false);
        }

        public void StopPlayer(bool isEnable = false)
        {
            if (!isEnable) soundController.SetBool(PlayerSoundType.FOOT, isEnable);
            playerRb.velocity = Vector2.zero;
            animator.SetBool("isMove", false);
        }

        public void Trigger()
        {
            curHealth = stat.HealthMax;
            curSkillGauge = 0f;
            changeListenerCaller.CallHpChangeListeners(curHealth);
            changeListenerCaller.CallShieldChangeListeners(curShield);
            changeListenerCaller.CallSkillGaugeChangeListeners(curSkillGauge);
            changeListenerCaller.CallPlayerSpawned();
        }
        
        public float CurrentHp => curHealth;
        public float CurrentSkillGauge => curSkillGauge;

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}
