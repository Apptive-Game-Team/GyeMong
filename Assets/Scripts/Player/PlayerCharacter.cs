using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

namespace playerCharacter
{
    public class PlayerCharacter : SingletonObject<PlayerCharacter>, IControllable, IEventTriggerable
    {
        [SerializeField] private float curHealth;
        public float maxHealth;
        public float attackPower;
        private bool isControlled = false;
        private Vector2 movement;
        private Vector2 lastMovementDirection;
        private Rigidbody2D playerRb;
        private Animator animator;
        private PlayerSoundController soundController;
        
        public GameObject attackColliderPrefab;

        public float moveSpeed = 2.0f;
        public float sprintSpeed = 4.0f;
        public float dashSpeed = 10.0f;

        private float dashDuration = 0.1f;
        private float dashDistance = 5.0f;
        private float dashCooldown = 1.0f;

        private float delayTime = 0.3f;

        private float parryTime = 0.5f;
        private float defendStartTime = 0f;
        private float blinkDelay = 0.2f;
        
        private float invincibilityDuration = 1.0f;

        private bool isDashing = false;
        private bool isAttacking = false;
        private bool isDefending = false;
        private bool canMove = true;
        private bool isInvincible = false;

        public bool isStartButton = false;
        public Material[] materials;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            soundController = GetComponent<PlayerSoundController>();

            attackPower = 1f;
            maxHealth = 1000f;
            curHealth = maxHealth;
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
            MoveCharacter();
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

            if (InputManager.Instance.GetKeyDown(ActionCode.Defend) && !isDefending)
            {
                StartCoroutine(Defend());
            }
        }

        private void MoveCharacter()
        {
            bool isRun = InputManager.Instance.GetKey(ActionCode.Run);
            float speed = isRun ? sprintSpeed : moveSpeed;
            soundController.SetRun(isRun);
            playerRb.velocity = movement * speed;

            animator.SetFloat("speed", speed);
        }

        private void UpdateState()
        {
            bool isMoving = movement.magnitude > 0;
            animator.SetBool("isMove", isMoving);
            soundController.SetBool(PlayerSoundType.FOOT, isMoving);

            if (isMoving)
            {
                lastMovementDirection = movement;
                animator.SetFloat("xDir", movement.x);
                animator.SetFloat("yDir", movement.y);
            }
            else
            {
                animator.SetFloat("xDir", lastMovementDirection.x);
                animator.SetFloat("yDir", lastMovementDirection.y);
            }
        }

        public void TakeDamage(float damage, bool isUnblockable = false)
        {
            if (isInvincible) return;
            
            StartCoroutine(EffectManager.Instance.ShakeCamera());
            
            if (!isUnblockable && isDefending)
            {
                
                if (Time.time - defendStartTime < parryTime) 
                {
                    soundController.Trigger(PlayerSoundType.SWORD_DEFEND_PERFECT);
                    damage = 0f;
                    Debug.Log($"Perfect Defend, damage : {damage}");
                    return;
                }
                else 
                {
                    soundController.Trigger(PlayerSoundType.SWORD_DEFEND_HIT);
                    damage /= 2f;
                    Debug.Log($"Defend, damage : {damage}");
                }
            }

            curHealth -= damage;
            //EffectManager.Instance.UpdateHpBar(curHealth);
            StartCoroutine(EffectManager.Instance.HurtEffect(1 - curHealth/maxHealth));
            
            if (curHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(TriggerInvincibility());
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
        
        private IEnumerator TriggerInvincibility()
        {
            isInvincible = true;

            // SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            // if (spriteRenderer != null)
            // {
            //     float elapsedTime = 0f;
            //     bool isVisible = true;

            //     while (elapsedTime < invincibilityDuration)
            //     {
            //         elapsedTime += 0.2f;
            //         isVisible = !isVisible;
            //         spriteRenderer.enabled = isVisible;
            //         yield return new WaitForSeconds(0.1f);
            //     }

            //     spriteRenderer.enabled = true;
            // }
            Material material = gameObject.GetComponent<Renderer>().material;
            material.SetFloat("_BlinkTrigger", 1f);
            yield return new WaitForSeconds(blinkDelay);
            material.SetFloat("_BlinkTrigger", 0f);
            yield return new WaitForSeconds(invincibilityDuration - blinkDelay);

            isInvincible = false;
        }

        private IEnumerator Dash()
        {
            movement = Vector2.zero;
            isDashing = true;
            canMove = false;
            animator.SetBool("isDashing", true);
            soundController.Trigger(PlayerSoundType.DASH);
            
            Vector2 dashDirection = lastMovementDirection.normalized;
            Vector2 startPosition = playerRb.position;
            Vector2 targetPosition = startPosition + dashDirection * dashDistance;

            float elapsedTime = 0f;

            while (elapsedTime < dashDuration)
            {
                elapsedTime += Time.deltaTime;

                /*playerRb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);*/
                playerRb.MovePosition(Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration));
                yield return null;
            }

            playerRb.velocity = Vector2.zero;
            yield return new WaitForSeconds(delayTime);

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

            yield return new WaitForSeconds(delayTime);


            animator.SetBool("isAttacking", false);
            canMove = true;
            isAttacking = false;
        }

        private IEnumerator Defend()
        {
            isDefending = true;
            canMove = false;
            animator.SetBool("isDefending", true);

            movement = Vector2.zero;
            playerRb.velocity = Vector2.zero;

            defendStartTime = Time.time;
            soundController.Trigger(PlayerSoundType.SWORD_DEFEND_START);

            yield return new WaitWhile(()=>InputManager.Instance.GetKey(ActionCode.Defend));

            animator.SetBool("isDefending", false);
            canMove = true;
            isDefending = false;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void SpawnAttackCollider()
        {
            Vector2 spawnPosition = playerRb.position + lastMovementDirection.normalized * 0.5f;

            float angle = Mathf.Atan2(lastMovementDirection.y, lastMovementDirection.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject attackCollider = Instantiate(attackColliderPrefab, spawnPosition, spawnRotation);
            attackCollider.GetComponent<AttackCollider>().Init(soundController);
            Destroy(attackCollider, delayTime);
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
            canMove = false; // ������ ����
            yield return new WaitForSeconds(duration); // ������ �ð� ���
            canMove = true; // ������ �簳
        }

        public Vector3 GetPlayerPosition()
        {
            return gameObject.transform.position;
        }

        public Vector2 GetPlayerDirection()
        {
            return lastMovementDirection;
        }

        public void LoadPlayerData()
        {
            PlayerData playerData = DataManager.Instance.LoadSection<PlayerData>("PlayerData");
            if (!playerData.isFirst && !isStartButton)
            {
                print("qwd");
                gameObject.transform.position = playerData.playerPosition;
                lastMovementDirection = playerData.playerDirection;
                StartCoroutine(LoadPlayerEffect());
            }
            isStartButton = false;
        }

        private IEnumerator LoadPlayerEffect()
        {
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
            isControlled = false;
        }

        public void Trigger()
        {
            curHealth = maxHealth;
            StartCoroutine(EffectManager.Instance.HurtEffect(1 - curHealth / maxHealth));
        }
    }
}
