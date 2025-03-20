using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Component.SkillIndicator
{
    public abstract class IndicatorBase : MonoBehaviour
    {
        [SerializeField] protected GameObject indicatorEffecterPrefab;
        protected float effectSpawnInterval = 0.4f;
        protected Transform indicator;
        protected SpriteRenderer spriteRenderer;
        protected Vector3 directionToTarget;
        public abstract void Initialize(Vector3 startPosition, Transform target);
        protected abstract void SetScale(float progress, float range);
        public IEnumerator GrowIndicator(Transform target, float range, float duration, float delay)
        {
            float elapsedTime = 0f;
            float effectElapsedTime = 0f;
            while (elapsedTime < duration)
            {
                directionToTarget = (target.position - transform.position).normalized;
                SetScale(elapsedTime / duration, range);
                indicator.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
                effectElapsedTime += Time.deltaTime;
                if (effectElapsedTime >= effectSpawnInterval)
                {
                    StartCoroutine(SpawnEffect(elapsedTime / duration, range));
                    effectElapsedTime = 0f;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(delay);
            Destroy(indicator.gameObject);
            Destroy(gameObject);
        }
        private IEnumerator SpawnEffect(float progress, float range)
        {
            GameObject effect = Instantiate(indicatorEffecterPrefab, indicator.position, indicator.rotation);
            Transform effectTransform = effect.transform;
            effectTransform.localScale = Vector3.zero;
            float elapsedTime = 0f;
            while (elapsedTime < effectSpawnInterval)
            {
                AdjustEffectScale(effectTransform, progress, range);
                effectTransform.rotation = indicator.rotation;
                effectTransform.position = indicator.position;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(effect);
        }
        protected abstract void AdjustEffectScale(Transform effectTransform, float progress, float range);
    }
}
