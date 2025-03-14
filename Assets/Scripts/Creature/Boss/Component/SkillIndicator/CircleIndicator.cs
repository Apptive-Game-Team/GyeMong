using Creature.Boss.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Creature.Boss.Component.SkillIndicator
{
    public class CircleIndicator : IndicatorBase
    {
        [SerializeField] protected GameObject circlePrefab;
        public override void Initialize(Vector3 startPosition, Vector3 direction, float range, float duration)
        {
            indicator = Instantiate(circlePrefab, startPosition, Quaternion.identity).transform;
        }
        public override IEnumerator GrowIndicator(float range, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float scaleX = Mathf.Lerp(0, range, elapsedTime / duration);
                indicator.localScale = new Vector3(scaleX, scaleX, 1);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            Destroy(indicator.gameObject);
        }
    }
}