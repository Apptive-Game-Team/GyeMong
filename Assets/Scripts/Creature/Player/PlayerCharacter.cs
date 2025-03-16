
using System;
using System.Collections;
using System.Event.Interface;
using Creature.Player.Component;
using Creature.Player.Component.Collider;
using UnityEngine;
using UnityEngine.Serialization;

namespace playerCharacter
{
    public class PlayerCharacter : SingletonObject<PlayerCharacter>, IControllable, IEventTriggerable, IBuffable
    {
        public PlayerChangeListenerCaller changeListenerCaller = new PlayerChangeListenerCaller();
        
        public StatComponent stat;
        public BuffComponent buffComponent;
        [SerializeField] private StatData _statData;
        [SerializeField] private float curHealth;
        [SerializeField] public float curShield;
        [SerializeField] private float curSkillGauge;
        public bool isControlled = false;
        private Vector2 movement;
        private Vector2 lastMovementDirection;
        private Vector2 mousePosition;
        private Rigidbody2D playerRb;
        private Animator animator;
        private PlayerSoundController soundController;
        
        public GameObject attackColliderPrefab;
        public GameObject skillColliderPrefab;
        private float blinkDelay = 0.2f;

        private bool isMoving = false;

        private bool isDashing = false;
        private bool isAttacking = false;
        private bool canMove = true;
        private bool isInvincible = false;


        public Material[] materials;

        protected override void Awake()
        {
            base.Awake();
            
            stat = _statData.GetStatComp();
            curHealth = stat.HealthMax;
            curSkillGauge = 0f;
        }

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            soundController = GetComponent<PlayerSoundController>();

            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material = materials[0];
        }

        private void Update()
        {
            if (!isControlled)
            {
                if (canMove)
                {
                    HandleInput();
                }
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

        private void HandleInput()
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

            if (InputManager.Instance.GetKeyDown(ActionCode.Dash) && !isDashing)
            {
                StartCoroutine(Dash());
            }

            if (InputManager.Instance.GetKeyDown(ActionCode.Attack) && !isAttacking)
            {
                StartCoroutine(Attack());
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
            Vector2 mouseDirection = (mousePosition - playerRb.position).normalized;

            if (!isDashing && isMoving)
            {
                lastMovementDirection = movement;
            }

            animator.SetFloat("xDir", mouseDirection.x);
            animator.SetFloat("yDir", mouseDirection.y);
        }

        public void TakeDamage(float damage, bool isUnblockable = false)
        {
            if (isInvincible) return;
            
            StartCoroutine(EffectManager.Instance.ShakeCamera());

            if (damage > curShield && curShield > 0)
            {
                curShield -= damage;
            }
            curHealth -= damage;
            changeListenerCaller.CallHpChangeListeners(curHealth);
            TakeGauge();
            StartCoroutine(EffectManager.Instance.HurtEffect(1 - curHealth/stat.HealthMax));
            
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
            isInvincible = true;
            
            Material material = gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_BlinkTrigger", 1f);
            yield return new WaitForSeconds(blinkDelay);
            material.SetFloat("_BlinkTrigger", 0f);
            yield return new WaitForSeconds(stat.InvincibilityDuration - blinkDelay);

            isInvincible = false;
        }

        private IEnumerator Dash()
        {
            isDashing = true;
            canMove = false;
            movement = Vector2.zero;
            animator.SetBool("isDashing", true);
            soundController.Trigger(PlayerSoundType.DASH);
            
            Vector2 dashDirection = lastMovementDirection.normalized;
            Vector2 startPosition = playerRb.position;

            RaycastHit2D hit = Physics2D.Raycast(startPosition, dashDirection, stat.DashDistance, LayerMask.GetMask("Wall"));
            Vector2 targetPosition = hit.collider == null ? startPosition + dashDirection * stat.DashDistance : hit.point + hit.normal * 0.1f;
            Debug.Log($"{startPosition} , {targetPosition} , {hit.collider}");

            float elapsedTime = 0f;

            while (elapsedTime < stat.DashDuration)
            {
                elapsedTime += Time.deltaTime;
                playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / stat.DashDuration));
                yield return null;
            }

            playerRb.velocity = Vector2.zero;

            canMove = true;
            animator.SetBool("isDashing", false);

            yield return new WaitForSeconds(stat.DashCooldown);

            isDashing = false;
        }

        private IEnumerator Attack()
        {
            soundController.Trigger(PlayerSoundType.SWORD_SWING);
            isAttacking = true;
            canMove = false;
            animator.SetBool("isAttacking", true);

            SpawnAttackCollider();

            movement = Vector2.zero;
            playerRb.velocity = Vector2.zero;

            canMove = true;
            
            yield return new WaitForSeconds(stat.AttackDelay);


            animator.SetBool("isAttacking", false);
            
            isAttacking = false;
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
            SpawnAttackCollider();
            SpawnSkillCollider();

            movement = Vector2.zero;
            playerRb.velocity = Vector2.zero;

            canMove = true;

            yield return new WaitForSeconds(stat.AttackDelay);

            animator.SetBool("isAttacking", false);
            
            isAttacking = false;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnAttackCollider()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mouseDirection = (mousePosition - playerRb.position).normalized;
            Vector2 spawnPosition = playerRb.position + mouseDirection * 0.5f;

            float angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject attackCollider = Instantiate(attackColliderPrefab, spawnPosition, spawnRotation, transform);
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
            try
            {
                GameObject.Find("PlayerGameOverEvent").gameObject.GetComponent<EventObject>().Trigger();
            }
            catch
            {
                Debug.Log("PlayerGameOverEvent not found");
            }
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
            playerRb.velocity = Vector2.zero;
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
            playerRb.velocity = Vector2.zero;
            animator.SetFloat("speed", speed);
            
            animator.SetBool("isMove", false);
            soundController.SetBool(PlayerSoundType.FOOT, false);
        }

        public void Trigger()
        {
            curHealth = stat.HealthMax;
            curSkillGauge = 0f;
            StartCoroutine(EffectManager.Instance.HurtEffect(1 - curHealth / stat.HealthMax));
        }
        
        public float CurrentHp { get { return curHealth; } }
    }
}
