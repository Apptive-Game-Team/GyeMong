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

            int afterImageCount = 0;
            int afterImageCountMax = 7;
            yield return (_dashTween = playerRb.DOMove(targetPosition, stat.DashDuration)
                .SetEase(Ease.OutCubic))
                .OnUpdate(() =>
                {
                    float progress = _dashTween.ElapsedPercentage(); // 0 ~ 70%
                    float targetThreshold = (afterImageCount + 1) / (float)afterImageCountMax * 0.7f;

                    if (progress >= targetThreshold && afterImageCount < afterImageCountMax)
                    {
                        SpawnAfterImage();
                        afterImageCount++;
                    }
                })
                .WaitForCompletion();

            StopPlayer();

            canMove = true;
            isDashing = false;
            animator.SetBool("isDashing", false);

            yield return new WaitForSeconds(stat.DashCooldown);
            canDash = true;
        }
        
        private void SpawnAfterImage()
        {
            GameObject clone = new GameObject("AfterImage")
            {
                transform =
                {
                    position = transform.position,
                    rotation = transform.rotation
                }
            };

            SpriteRenderer sr = clone.AddComponent<SpriteRenderer>();
            if (isInvincible)
            {
                sr.material = materials[2];
                sr.material.SetFloat("_isUsable", 1f);
            }
            sr.sprite = _spriteRenderer.sprite;
            sr.flipX = _spriteRenderer.flipX;
            sr.sortingLayerID = _spriteRenderer.sortingLayerID;
            sr.sortingOrder = _spriteRenderer.sortingOrder - 1;
            sr.color = new Color(1f, 1f, 1f, 0.45f);
            
            Destroy(clone, stat.DashDuration * 0.3f);
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
            if (ReadHeldAction(ActionCode.MoveUp)) dir += Vector2.up;
            if (ReadHeldAction(ActionCode.MoveDown)) dir += Vector2.down;
            if (ReadHeldAction(ActionCode.MoveRight)) dir += Vector2.right;
            if (ReadHeldAction(ActionCode.MoveLeft)) dir += Vector2.left;

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
            while (ReadHeldAction(ActionCode.Skill) == true && curChargePower < _chargeThreshold)
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

            while (ReadHeldAction(ActionCode.Heal))
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
            Vector2 mousePosition = ReadAimPosition();
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
            Vector2 mousePosition = ReadAimPosition();
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

    }
}
