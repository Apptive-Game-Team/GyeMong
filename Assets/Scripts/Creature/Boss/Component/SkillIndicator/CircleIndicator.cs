using Creature.Boss.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Creature.Boss.Component.SkillIndicator
{
    public class CircleIndicator : IndicatorBase
    {
        [SerializeField] private GameObject circlePrefab;
        SpriteRenderer circleSpriteRenderer;
        public override void Initialize(Vector3 startPosition, Transform target, float range, float duration)
        {
            indicator = Instantiate(circlePrefab, startPosition, Quaternion.identity).transform;
            circleSpriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        public override IEnumerator GrowIndicator(Vector3 startPosition, Transform target, float range, float duration)
        {
            float elapsedTime = 0f;
            float effectElapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float radius = Mathf.Lerp(0, range, elapsedTime / duration);
                indicator.localScale = new Vector3(radius, radius, 1);
                effectElapsedTime += Time.deltaTime;
                if (effectElapsedTime >= effectSpawnInterval)
                {
                    StartCoroutine(SpawnEffect(radius, range));
                    effectElapsedTime = 0f;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Color lineColor = circleSpriteRenderer.color;
            lineColor.a = 1f;
            circleSpriteRenderer.color = lineColor;
            yield return new WaitForSeconds(0.4f);
            Destroy(indicator.gameObject);
            Destroy(gameObject);
        }
        private IEnumerator SpawnEffect(float radius, float range)
        {
            GameObject effect = Instantiate(indicatorEffecterPrefab, indicator.position, indicator.rotation);
            Transform effectTransform = effect.transform;
            effectTransform.localScale = new Vector3(0, 0, 1);
            float elapsedTime = 0f;
            while (elapsedTime < effectSpawnInterval)
            {
                if (range <= radius + 1)
                    radius = range - 1;
                float effectScaleX = Mathf.Lerp(radius + 1, radius + 1, elapsedTime / effectSpawnInterval);
                effectTransform.localScale = new Vector3(effectScaleX, effectScaleX, 1);
                effectTransform.position = indicator.position;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(effect);
        }
    }
}