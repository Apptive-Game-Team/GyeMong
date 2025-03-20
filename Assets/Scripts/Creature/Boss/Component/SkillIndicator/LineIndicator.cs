using Creature.Boss.Component;
using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Creature.Boss.Component.SkillIndicator
{
    public class LineIndicator : IndicatorBase
    {
        [SerializeField] private GameObject linePrefab;
        SpriteRenderer lineSpriteRenderer;
        public override void Initialize(Vector3 startPosition, Transform target)
        {
            directionToTarget = (target.position - startPosition).normalized;
            indicator = Instantiate(linePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, directionToTarget)).transform;
            lineSpriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        public override IEnumerator GrowIndicator(Vector3 startPosition, Transform target, float range, float duration)
        {
            float elapsedTime = 0f;
            float effectElapsedTime = 0f;
            while (elapsedTime < duration)
            {
                directionToTarget = (target.position - transform.position).normalized;
                float scaleY = Mathf.Lerp(0, range, elapsedTime / duration);
                indicator.localScale = new Vector3(0.5f, scaleY, 1);
                indicator.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
                effectElapsedTime += Time.deltaTime;
                if (effectElapsedTime >= effectSpawnInterval)
                {
                    StartCoroutine(SpawnEffect(scaleY, range));
                    effectElapsedTime = 0f;
                }
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Color lineColor = lineSpriteRenderer.color;
            lineColor.a = 1f;
            lineSpriteRenderer.color = lineColor;
            yield return new WaitForSeconds(0.4f);
            Destroy(indicator.gameObject);
            Destroy(gameObject);
        }
        private IEnumerator SpawnEffect(float scaleY, float range)
        {
            GameObject effect = Instantiate(indicatorEffecterPrefab, indicator.position, indicator.rotation);
            Transform effectTransform = effect.transform;
            effectTransform.localScale = new Vector3(0.5f, 0, 1);
            float elapsedTime = 0f;
            while (elapsedTime < effectSpawnInterval)
            {
                if (range <= scaleY + 1)
                    scaleY = range - 1;
                float effectScaleY = Mathf.Lerp(0, scaleY + 1, elapsedTime / effectSpawnInterval);
                effectTransform.localScale = new Vector3(0.5f, effectScaleY, 1);
                effectTransform.rotation = indicator.rotation;
                effectTransform.position = indicator.position;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(effect);
        }
    }
}