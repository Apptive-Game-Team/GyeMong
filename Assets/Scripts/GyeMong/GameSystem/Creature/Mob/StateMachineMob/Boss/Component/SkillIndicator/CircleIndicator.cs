using UnityEngine;
using System.Collections;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator
{
    public class CircleIndicator : IndicatorBase
    {
        [SerializeField] private GameObject circlePrefab;
        public override void Initialize(Vector3 startPosition, Transform target)
        {
            indicator = Instantiate(circlePrefab, startPosition, Quaternion.identity).transform;
            spriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        protected override void SetScale(float progress, float range)
        {
            float radius = Mathf.Lerp(0, range, progress);
            indicator.localScale = new Vector3(2 * radius, 2 * radius, 1);
        }
        public override IEnumerator GrowIndicator(Transform target, float duration, float delay, float radius)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                directionToTarget = (target.position - transform.position).normalized;
                SetScale(elapsedTime / duration, radius);
                indicator.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Color color = spriteRenderer.color;
            color.a = 0.8f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(delay);
            Destroy(indicator.gameObject);
            Destroy(gameObject);
        }
    }
}