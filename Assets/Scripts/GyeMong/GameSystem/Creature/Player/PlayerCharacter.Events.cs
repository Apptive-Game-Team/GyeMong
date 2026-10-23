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
        public void SetPlayerMove(bool _canMove)
        {
            canMove = _canMove;
            if (!canMove)
            {
                movement = Vector2.zero;
                if (playerRb != null) playerRb.velocity = Vector2.zero;
            }
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
