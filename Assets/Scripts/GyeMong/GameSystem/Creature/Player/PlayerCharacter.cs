using System.Collections;
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
        private float blinkDelay = 0.2f;

        private bool isMoving = false;

        public bool isDashing = false;
        private bool isAttacking = false;
        private bool canMove = true;
        private bool isInvincible = false;
        private bool canCombo = false;
        private bool comboQueued = false;

        public bool isTutorial;

        public Material[] materials;

        protected void Awake()
        {
            stat = _statData.GetStatComp();
            curHealth = stat.HealthMax;
            curSkillGauge = 0f;
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            soundController = GetComponent<PlayerSoundController>();
            isTutorial = PlayerPrefs.GetInt("TutorialFlag") == 0;
        }

        private void Start()
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material = materials[0];
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
                }
                HandleActionInput();
                UpdateState();
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
            if (InputManager.Instance.GetKeyDown(ActionCode.Dash) && !isDashing)
            {
                StartCoroutine(Dash());
            }

            if (InputManager.Instance.GetKeyDown(ActionCode.Attack))
            {
                if (!isAttacking)
                {
                    StartCoroutine(Attack());
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
        }

        private void MoveCharacter()
        {
            soundController.SetRun(isMoving);
            playerRb.velocity = movement * stat.MoveSpeed;
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
            if (isInvincible) return;
            
            SceneContext.CameraManager.CameraShake(0.1f);

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

        public void Heal(float amount)
        {
            curHealth += amount;
            if (curHealth > stat.HealthMax)
            {
                curHealth = stat.HealthMax;
            }
            changeListenerCaller.CallHpChangeListeners(curHealth);
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
            Material material = gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_BlinkTrigger", 1f);
            yield return new WaitForSeconds(blinkDelay);
            material.SetFloat("_BlinkTrigger", 0f);
            yield return new WaitForSeconds(stat.InvincibilityDuration - blinkDelay);
        }

        private IEnumerator Dash()
        {
            isDashing = true;
            canMove = false;
            movement = Vector2.zero;
            animator.SetBool("isDashing", true);
            soundController.Trigger(PlayerSoundType.DASH);

            Vector2 dashDirection = GetCurrentInputDirection(lastMovementDirection.normalized);
            Vector2 startPosition = playerRb.position;

            RaycastHit2D hit = Physics2D.Raycast(startPosition, dashDirection, stat.DashDistance,
                LayerMask.GetMask("Wall"));
            Vector2 targetPosition = hit.collider == null
                ? startPosition + dashDirection * stat.DashDistance
                : hit.point + hit.normal * 0.1f;

            float elapsedTime = 0f;

            while (elapsedTime < stat.DashDuration)
            {
                elapsedTime += Time.deltaTime;
                playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / stat.DashDuration));
                yield return null;
            }

            StopPlayer();

            canMove = true;
            animator.SetBool("isDashing", false);

            yield return new WaitForSeconds(stat.DashCooldown);

            isDashing = false;
        }

        private Vector2 GetCurrentInputDirection(Vector2 direction)
        {
            var dir = Vector2.zero;
            if (InputManager.Instance.GetKey(ActionCode.MoveUp)) dir += Vector2.up;
            if (InputManager.Instance.GetKey(ActionCode.MoveDown)) dir += Vector2.down;
            if (InputManager.Instance.GetKey(ActionCode.MoveRight)) dir += Vector2.right;
            if (InputManager.Instance.GetKey(ActionCode.MoveLeft)) dir += Vector2.left;

            if (dir != Vector2.zero) return dir;
            return direction;
        }

        private IEnumerator Attack()
        {
            isAttacking = true;
            canMove = false;
            
            movement = Vector2.zero;
            StopPlayer();
            
            AttackMove(GetCurrentInputDirection(mouseDirection));
            yield return new WaitForSeconds(stat.AttackDelay / 2);
            
            soundController.Trigger(PlayerSoundType.SWORD_SWING);

            animator.SetBool("isAttacking", true);

            SpawnAttackCollider(attackColliderPrefab);
            
            canCombo = true;
            yield return new WaitForSeconds(stat.AttackDelay); // 콤보 입력 대기
            canCombo = false;

            if (comboQueued)
            {
                comboQueued = false;
                AttackMove(GetCurrentInputDirection(mouseDirection));
                yield return new WaitForSeconds(stat.AttackDelay / 2);
                soundController.Trigger(PlayerSoundType.SWORD_SWING);
                animator.SetBool("isAttacking2", true);
                SpawnAttackCollider(attackComboColliderPrefab);
                yield return new WaitForSeconds(stat.AttackDelay);
                animator.SetBool("isAttacking2", false);
            }
            
            canMove = true;
            animator.SetBool("isAttacking", false);
            isAttacking = false;
        }

        private void AttackMove(Vector2 direction)
        {
            transform.DOMove((Vector2)transform.position + direction / 3, stat.AttackDelay).SetEase(Ease.OutQuad);
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
            StageManager.LoseStage(this);
            // try
            // {
            //     GameObject.Find("PlayerGameOverEvent").gameObject.GetComponent<EventObject>().Trigger();
            // }
            // catch
            // {
            //     Debug.Log("PlayerGameOverEvent not found");
            // }
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
