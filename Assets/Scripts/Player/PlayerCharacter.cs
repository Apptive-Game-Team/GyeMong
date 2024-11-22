using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace playerCharacter
{
    public class PlayerCharacter : SingletonObject<PlayerCharacter>
    {
        [SerializeField] public float curHealth;
        public float maxHealth;
        public float attackPower;

        private Vector2 movement;
        private Vector2 lastMovementDirection;
        private Rigidbody2D playerRb;
        private Animator animator;

        public GameObject attackColliderPrefab;

        public float moveSpeed = 2.0f;
        public float sprintSpeed = 4.0f;
        public float dashSpeed = 10.0f;

        private float dashDuration = 0.1f;
        private float dashDistance = 5.0f;
        private float dashCooldown = 1.0f;

        private float delayTime = 0.3f;

        private float defendTime = 1.0f;
        private float defendStartTime = 0f;
        
        private float invincibilityDuration = 3.0f;

        private bool isDashing = false;
        private bool isAttacking = false;
        private bool isDefending = false;
        private bool canMove = true;
        private bool isInvincible = false;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            attackPower = 1f;
            maxHealth = 1000f;
            curHealth = maxHealth;
        }

        private void Update()
        {
            if (canMove)
            {
                HandleInput();
            }
            UpdateState();

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
            float speed = InputManager.Instance.GetKey(ActionCode.Run) ? sprintSpeed : moveSpeed;
            playerRb.velocity = movement * speed;

            animator.SetFloat("speed", speed);
        }

        private void UpdateState()
        {
            bool isMoving = movement.magnitude > 0;
            animator.SetBool("isMove", isMoving);

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

        public void TakeDamage(float damage)
        {
            if (isInvincible) return;

            if (isDefending)
            {
                if (Time.time - defendStartTime < defendTime / 2f) 
                {
                    damage = 0f;
                    Debug.Log($"Perfect Defend, damage : {damage}");
                }
                else 
                {
                    damage /= 2f;
                    Debug.Log($"�ϴ� Defend, damage : {damage}");
                }
            }

            curHealth -= damage;

            if (curHealth <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(TriggerInvincibility());
            }
        }

        private IEnumerator TriggerInvincibility()
        {
            Debug.Log("���� ����");
            isInvincible = true;

            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                float elapsedTime = 0f;
                bool isVisible = true;

                while (elapsedTime < invincibilityDuration)
                {
                    elapsedTime += 0.2f;
                    isVisible = !isVisible;
                    spriteRenderer.enabled = isVisible;
                    yield return new WaitForSeconds(0.1f);
                }

                spriteRenderer.enabled = true;
            }

            isInvincible = false;
            Debug.Log("���� ����");
        }

        private IEnumerator Dash()
        {
            movement = Vector2.zero;
            isDashing = true;
            canMove = false;
            animator.SetBool("isDashing", true);

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

            yield return new WaitForSeconds(defendTime);

            animator.SetBool("isDefending", false);
            canMove = true;
            isDefending = false;
        }

        private void SpawnAttackCollider()
        {
            Vector2 spawnPosition = playerRb.position + lastMovementDirection.normalized * 0.5f;

            float angle = Mathf.Atan2(lastMovementDirection.y, lastMovementDirection.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

            GameObject attackCollider = Instantiate(attackColliderPrefab, spawnPosition, spawnRotation);

            Destroy(attackCollider, delayTime);
        }

        private void Die()
        {
            Destroy(gameObject);
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
    }
}
