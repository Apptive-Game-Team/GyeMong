using Creature.Boss.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Creature.Boss.Component.SkillIndicator
{
    public class ConeIndicator : IndicatorBase
    {
        [SerializeField] private GameObject conePrefab;
        SpriteRenderer coneSpriteRenderer;
        public override void Initialize(Vector3 startPosition, Transform target)
        {
            directionToTarget = (target.position - startPosition).normalized;
            indicator = Instantiate(conePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, directionToTarget)).transform;
            coneSpriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        public override IEnumerator GrowIndicator(Vector3 startPosition, Transform target, float range, float duration)
        {
            float elapsedTime = 0f;
            float effectElapsedTime = 0f;
            while (elapsedTime < duration)
            {
                directionToTarget = (target.position - transform.position).normalized;
                float scaleX = Mathf.Lerp(0, range, elapsedTime / duration);
                indicator.localScale = new Vector3(scaleX, 0.5f, 1);
                indicator.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
                effectElapsedTime += Time.deltaTime;
                if (effectElapsedTime >= effectSpawnInterval)
                {
                    StartCoroutine(SpawnEffect(scaleX, range));
                    effectElapsedTime = 0f;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Color lineColor = coneSpriteRenderer.color;
            lineColor.a = 1f;
            coneSpriteRenderer.color = lineColor;
            yield return new WaitForSeconds(0.4f);
            Destroy(indicator.gameObject);
            Destroy(gameObject);
        }
        private IEnumerator SpawnEffect(float scaleX, float range)
        {
            GameObject effect = Instantiate(indicatorEffecterPrefab, indicator.position, indicator.rotation);
            Transform effectTransform = effect.transform;
            effectTransform.localScale = new Vector3(0, 0.5f, 1);
            float elapsedTime = 0f;
            while (elapsedTime < effectSpawnInterval)
            {
                if (range <= scaleX + 1)
                    scaleX = range - 1;
                float effectScaleX = Mathf.Lerp(0, scaleX + 1, elapsedTime / effectSpawnInterval);
                effectTransform.localScale = new Vector3(effectScaleX, 0.5f, 1);
                effectTransform.rotation = indicator.rotation;
                effectTransform.position = indicator.position;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(effect);
        }
    }
}