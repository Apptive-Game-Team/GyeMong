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
                float scaleX = Mathf.Lerp(0, range, elapsedTime / duration);
                indicator.localScale = new Vector3(scaleX, scaleX, 1);
                effectElapsedTime += Time.deltaTime;
                if (effectElapsedTime >= effectSpawnInterval)
                {
                    StartCoroutine(SpawnEffect(scaleX, range));
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
        private IEnumerator SpawnEffect(float scaleX, float range)
        {
            GameObject effect = Instantiate(indicatorEffecterPrefab, indicator.position, indicator.rotation);
            Transform effectTransform = effect.transform;
            effectTransform.localScale = new Vector3(0, 0, 1);
            float elapsedTime = 0f;
            while (elapsedTime < effectSpawnInterval)
            {
                if (range <= scaleX + 1)
                    scaleX = range - 1;
                float effectScaleX = Mathf.Lerp(scaleX + 1, scaleX + 1, elapsedTime / effectSpawnInterval);
                effectTransform.localScale = new Vector3(effectScaleX, effectScaleX, 1);
                effectTransform.position = indicator.position;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(effect);
        }
    }
}