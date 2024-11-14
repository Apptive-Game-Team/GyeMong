using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace playerCharacter
{
    public class PlayerCharacter : SingletonObject<PlayerCharacter>
    {
        public float moveSpeed = 2.0f;
        public float sprintSpeed = 4.0f;
        public float dashSpeed = 10.0f;
        public float dashDuration = 0.1f;
        public float dashDistance = 5.0f;
        public float delayTime = 0.5f;

        private Vector2 movement;
        private Vector2 lastMovementDirection;
        private Rigidbody2D playerRb;
        private Animator animator;
        private bool isDashing = false;
        private bool isAttacking = false;
        private bool isDefending = false;
        private bool canMove = true;

        private void Start()
        {
            playerRb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
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

            if (Input.GetKey(KeyCode.RightArrow))
            {
                movement.x = 1;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                movement.x = -1;
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                movement.y = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                movement.y = -1;
            }

            movement.Normalize();

            if (Input.GetKeyDown(KeyCode.X) && !isDashing)
            {
                StartCoroutine(Dash());
            }

            if (Input.GetKeyDown(KeyCode.A) && !isAttacking)
            {
                StartCoroutine(Attack());
            }

            if (Input.GetKeyDown(KeyCode.S) && !isDefending)
            {
                StartCoroutine(Defend());
            }
        }

        private void MoveCharacter()
        {
            float speed = Input.GetKey(KeyCode.Z) ? sprintSpeed : moveSpeed;
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
  
                playerRb.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
                yield return null;
            }

            playerRb.velocity = Vector2.zero;
            yield return new WaitForSeconds(delayTime);
            isDashing = false;
            canMove = true;
            animator.SetBool("isDashing", false);
        }

        private IEnumerator Attack()
        {
            isAttacking = true;
            canMove = false;
            animator.SetBool("isAttacking", true);

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
            yield return new WaitForSeconds(delayTime);

            animator.SetBool("isDefending", false);
            canMove = true;
            isDefending = false;
        }
    }
}
