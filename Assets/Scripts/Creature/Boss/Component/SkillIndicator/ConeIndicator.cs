using Creature.Boss.Component;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Creature.Boss.Component.SkillIndicator
{
    public class ConeIndicator : IndicatorBase
    {
        [SerializeField] private GameObject conePrefab;
        public override void Initialize(Vector3 startPosition, Transform target, float range, float duration)
        {
            directionToTarget = (target.position - startPosition).normalized;
            indicator = Instantiate(conePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, directionToTarget)).transform;
        }
        public override IEnumerator GrowIndicator(Vector3 startPosition, Transform target, float range, float duration)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                directionToTarget = (target.position - transform.position).normalized;
                float scaleX = Mathf.Lerp(0, range, elapsedTime / duration);
                indicator.localScale = new Vector3(scaleX, 0.5f, 1);
                indicator.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
            Destroy(indicator.gameObject);
        }
    }
}