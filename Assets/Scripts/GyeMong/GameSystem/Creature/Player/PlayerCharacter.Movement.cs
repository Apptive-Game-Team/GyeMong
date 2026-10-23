using System.Collections;
using GyeMong.EventSystem.Interface;
using GyeMong.GameSystem.Creature.Player.Component;
using GyeMong.GameSystem.Creature.Player.Component.Collider;
using GyeMong.GameSystem.Creature.Player.Controller;
using GyeMong.InputSystem;
using UnityEngine;
using DG.Tweening;

namespace GyeMong.GameSystem.Creature.Player
{
    public partial class PlayerCharacter
    {
        private bool ReadHeldAction(ActionCode action)
        {
#if ARTEL_GAME_FUNCTIONS && (UNITY_EDITOR || (DEVELOPMENT_BUILD && !UNITY_WEBGL))
            if (_qaInputActive) return QaActionAllowed(action) && _qaHeld.Contains(action);
#endif
            return InputManager.Instance.GetKey(action);
        }

        private bool ReadPressedAction(ActionCode action)
        {
#if ARTEL_GAME_FUNCTIONS && (UNITY_EDITOR || (DEVELOPMENT_BUILD && !UNITY_WEBGL))
            if (_qaInputActive) return QaActionAllowed(action) && _qaPressed.Remove(action);
#endif
            return InputManager.Instance.GetKeyDown(action);
        }

        private Vector3 ReadAimPosition()
        {
#if ARTEL_GAME_FUNCTIONS && (UNITY_EDITOR || (DEVELOPMENT_BUILD && !UNITY_WEBGL))
            if (_qaInputActive) return _qaAim;
#endif
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        private void HandleMoveInput()
        {
            movement.x = 0;
            movement.y = 0;

            if (ReadHeldAction(ActionCode.MoveRight))
            {
                movement.x = 1;
            }
            else if (ReadHeldAction(ActionCode.MoveLeft))
            {
                movement.x = -1;
            }

            if (ReadHeldAction(ActionCode.MoveUp))
            {
                movement.y = 1;
            }
            else if (ReadHeldAction(ActionCode.MoveDown))
            {
                movement.y = -1;
            }

            movement.Normalize();
        }

        private void HandleActionInput()
        {
            if (ReadPressedAction(ActionCode.Dash) && canDash)
            {
                StartCoroutine(Dash());
            }

            if ((ReadPressedAction(ActionCode.Attack) || ReadHeldAction(ActionCode.Attack)) &&
                !isDashing)
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

            if (ReadPressedAction(ActionCode.Skill) && !isAttacking && curSkillGauge >= stat.SkillCost)
            {
                StartCoroutine(ChargeSkillAttack());
                // StartCoroutine(SkillAttack());
            }

            if (ReadPressedAction(ActionCode.Heal) && !isAttacking && curSkillGauge >= stat.HealCost && curHealth != stat.HealthMax)
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

            mousePosition = ReadAimPosition();
            mouseDirection = (mousePosition - playerRb.position).normalized;

            if (canMove && isMoving)
            {
                lastMovementDirection = movement;
            }

            animator.SetFloat("xDir", mouseDirection.x);
            animator.SetFloat("yDir", mouseDirection.y);
        }

    }
}
