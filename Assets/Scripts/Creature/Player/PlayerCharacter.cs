
using System;
using System.Collections;
using UnityEngine;

namespace playerCharacter
{
    public class PlayerCharacter : SingletonObject<PlayerCharacter>, IControllable, IEventTriggerable
    {
        [SerializeField] private float curHealth;
        public float maxHealth;
        [SerializeField] private float curSkillGauge;
        public float GetCurSkillGauge() { return curSkillGauge; }
        public float maxSkillGauge;
        public float gaugeIncreaseValue;
        public float attackGaugeIncreaseValue;
        public float skillUsageGauge = 30f;
        public float attackPower;
        public bool isControlled = false;
        private Vector2 movement;
        private Vector2 lastMovementDirection;
        private Vector2 mousePosition;
        private Rigidbody2D playerRb;
        private Animator animator;
        private PlayerSoundController soundController;
        
        public GameObject attackColliderPrefab;
        public GameObject skillColliderPrefab;

        public float moveSpeed = 4.0f;
        public float dashSpeed = 10.0f;
        public float skillSpeed = 10.0f;

        private float dashDuration = 0.1f;
        private float dashDistance = 5.0f;
        private float dashCooldown = 1.0f;

        private float delayTime = 0.2f;

        private float parryTime = 0.5f;
        private float blinkDelay = 0.2f;
        
        private float invincibilityDuration = 1.0f;
        private bool isMoving = false;

        private bool isDashing = false;
        private bool isAttacking = false;
        private bool canMove = true;
        private bool isInvincible = false;


        public Material[] materials;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            soundController = GetComponent<PlayerSoundController>();

            attackPower = 1f;
            maxHealth = 1000f;
            curHealth = maxHealth;
            maxSkillGauge = 100f;
            curSkillGauge = 0f;
            gaugeIncreaseValue = 10f;
            attackGaugeIncreaseValue = 2f;
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

            if (InputManager.Instance.GetKeyDown(ActionCode.Skill) && !isAttacking && curSkillGauge >= skillUsageGauge)
            {
                StartCoroutine(SkillAttack());
            }
        }

        private void MoveCharacter()
        {
            soundController.SetRun(isMoving);
            playerRb.velocity = movement * moveSpeed;
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

            curHealth -= damage;
            TakeGauge();
            StartCoroutine(EffectManager.Instance.HurtEffect(1 - curHealth/maxHealth));
            
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
            curSkillGauge -= gaugeIncreaseValue;
            if (curSkillGauge < 0)
            {
                curSkillGauge = 0f;
            }
        }

        public void AttackIncreaseGauge()
        {
            curSkillGauge += attackGaugeIncreaseValue;
            if (curSkillGauge > maxSkillGauge)
            {
                curSkillGauge = maxSkillGauge;
            }
        }

        public void Heal(float amount)
        {
            curHealth += amount;
            if (curHealth > maxHealth)
            {
                curHealth = maxHealth;
            }
        }

        public void GrazeIncreaseGauge(float ratio)
        {
            soundController.Trigger(PlayerSoundType.GRAZE);
            curSkillGauge += gaugeIncreaseValue / ratio;
            if (curSkillGauge > maxSkillGauge)
            {
                curSkillGauge = maxSkillGauge;
            }
        }
        
        private IEnumerator TriggerInvincibility()
        {
            isInvincible = true;
            
            Material material = gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_BlinkTrigger", 1f);
            yield return new WaitForSeconds(blinkDelay);
            material.SetFloat("_BlinkTrigger", 0f);
            yield return new WaitForSeconds(invincibilityDuration - blinkDelay);

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

            RaycastHit2D hit = Physics2D.Raycast(startPosition, dashDirection, dashDistance, LayerMask.GetMask("Wall"));
            Vector2 targetPosition = hit.collider == null ? startPosition + dashDirection * dashDistance : hit.point + hit.normal * 0.1f;
            Debug.Log($"{startPosition} , {targetPosition} , {hit.collider}");

            float elapsedTime = 0f;

            while (elapsedTime < dashDuration)
            {
                elapsedTime += Time.deltaTime;
                playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration));
                yield return null;
            }

            playerRb.velocity = Vector2.zero;

            canMove = true;
            animator.SetBool("isDashing", false);

            yield return new WaitForSeconds(dashCooldown);

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
            
            yield return new WaitForSeconds(delayTime);


            animator.SetBool("isAttacking", false);
            
            isAttacking = false;
        }

        private IEnumerator SkillAttack()
        {
            soundController.Trigger(PlayerSoundType.SWORD_SKILL);
            isAttacking = true;
            canMove = false;
            animator.SetBool("isAttacking", true);

            curSkillGauge -= skillUsageGauge;
            SpawnAttackCollider();
            SpawnSkillCollider();

            movement = Vector2.zero;
            playerRb.velocity = Vector2.zero;

            canMove = true;

            yield return new WaitForSeconds(delayTime);

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
            Destroy(attackCollider, delayTime);
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
            skillRigidbody.velocity = mouseDirection.normalized * skillSpeed;

            attackCollider.GetComponent<AttackCollider>().Init(soundController);
            Destroy(attackCollider, delayTime * 2);
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
            playerRb.velocity = movement * speed;
            animator.SetFloat("speed", speed);
            
            animator.SetBool("isMove", false);
            soundController.SetBool(PlayerSoundType.FOOT, false);
        }

        public void Trigger()
        {
            curHealth = maxHealth;
            curSkillGauge = 0f;
            StartCoroutine(EffectManager.Instance.HurtEffect(1 - curHealth / maxHealth));
        }
        
        public float CurrentHp { get { return curHealth; } }
        public float MaxHp { get { return maxHealth; } }
    }
}
