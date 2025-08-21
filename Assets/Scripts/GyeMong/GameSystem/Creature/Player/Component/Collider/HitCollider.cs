using System;
using System.Collections;
using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Attack;
using GyeMong.GameSystem.Creature.Attack.Component;
using GyeMong.SoundSystem;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Player.Component.Collider
{
    public class HitCollider : MonoBehaviour
    {
        [SerializeField] private AirborneController airborneController;

        private Dictionary<Collider2D, float> multiHitTimers = new Dictionary<Collider2D, float>();

        private void OnTriggerEnter2D(Collider2D other) => HandleAttackCollision(other);
        private void OnTriggerStay2D(Collider2D other) => HandleAttackCollision(other);
        private void OnCollisionEnter2D(Collision2D other) => HandleAttackCollision(other.collider);
        private void OnCollisionStay2D(Collision2D other) => HandleAttackCollision(other.collider);

        private void HandleAttackCollision(Collider2D collider)
        {
            if (!collider.CompareTag("EnemyAttack")) return;
            if (SceneContext.Character.isInvincible) return;

            EnemyAttackInfo enemyAttackInfo = collider.GetComponent<AttackObjectController>()?.AttackInfo;
            if (enemyAttackInfo == null) return;
            AttackObjectController attackObjectController = collider.GetComponent<AttackObjectController>();

            if (enemyAttackInfo.canMultiHit)
            {
                
                attackObjectController.isAttacked = true;
                float lastHitTime = multiHitTimers.ContainsKey(collider) ? multiHitTimers[collider] : 0f;

                if (Time.time < lastHitTime + enemyAttackInfo.multiHitDelay) return;

                multiHitTimers[collider] = Time.time;
                ApplyHitImpact(enemyAttackInfo.damage, collider, true);
                SceneContext.Character.TakeDamage(enemyAttackInfo.damage);
            }
            else if (!attackObjectController.isAttacked)
            {
                ApplyHitImpact(enemyAttackInfo.damage, collider);
                attackObjectController.isAttacked = true;

                if (enemyAttackInfo.soundObject != null)
                    enemyAttackInfo.soundObject.PlayAsync();

                SceneContext.Character.TakeDamage(enemyAttackInfo.damage);
                collider.gameObject.SetActive(!enemyAttackInfo.isDestroyOnHit);
            }
        }

        private void ApplyAirborne(AttackObjectController controller, float knockbackSpeed)
        {
            if (controller.AttackInfo.knockbackAmount > 0)
            {
                Vector3 origin = controller.gameObject.transform.position;
                Vector3 direction = (SceneContext.Character.transform.position - origin).normalized;
                SceneContext.Character.isControlled = true;
                StartCoroutine(ActionAfter(airborneController.AirborneTo(direction * controller.AttackInfo.knockbackAmount + SceneContext.Character.transform.position, 1f, knockbackSpeed),
                    () => { SceneContext.Character.isControlled = false; }));
            }
        }

        private IEnumerator ActionAfter(IEnumerator coroutine, Action action)
        {
            yield return StartCoroutine(coroutine);
            action?.Invoke();
        }

        private void ApplyHitImpact(float damage, Collider2D collider, bool isMultiHit = false)
        {
            float baseDamage = SceneContext.Character.stat.HealthMax / 3;
            float ratio = damage / baseDamage;

            // 히트스탑
            float baseHitStopRatio = 0.2f;
            float hitStopRatio = baseHitStopRatio * ratio;
            hitStopRatio = 1 - Mathf.Clamp(hitStopRatio, 0.1f, 0.3f);

            float baseHitStopDuration = 0.3f;
            float hitStopDuration = baseHitStopDuration * ratio;
            hitStopDuration = Mathf.Clamp(hitStopDuration, 0.1f, 0.5f);

            // 카메라 셰이크
            float baseShakePower = 0.1f;
            float shakePower = baseShakePower * ratio;
            shakePower = Mathf.Clamp(shakePower, 0.05f, 0.15f);

            // 넉백 스피드
            float baseKnockbackSpeed = 9f;
            float knockbackSpeed = Mathf.Min(baseKnockbackSpeed / ratio, 8f);
            knockbackSpeed = Mathf.Clamp(knockbackSpeed, 8f, 10f);

            // 타격음 볼륨 비율
            float baseVolumeRatio = 1f;
            float volumeRatio = baseVolumeRatio * ratio;
            volumeRatio = Mathf.Clamp(volumeRatio, 0.7f, 1.3f);
            
            if (!isMultiHit) StartCoroutine(HitStop(hitStopRatio, hitStopDuration));
            SceneContext.CameraManager.CameraShake(shakePower);
            if (!isMultiHit) ApplyAirborne(collider.GetComponent<AttackObjectController>(), knockbackSpeed);
            HitSound(volumeRatio);
        }

        private IEnumerator HitStop(float hitStopRatio, float hitStopDuration)
        {
            Time.timeScale = hitStopRatio;
            yield return new WaitForSeconds(hitStopDuration);
            Time.timeScale = 1f;
        }

        private void HitSound(float volumeRatio)
        {
            Sound.Play($"EFFECT_Player_Hit_{UnityEngine.Random.Range(1, 4)}", false, volumeRatio);
        }
    }
}
