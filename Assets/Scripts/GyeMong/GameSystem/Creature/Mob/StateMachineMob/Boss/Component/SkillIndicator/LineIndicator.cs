using UnityEngine;
using System.Collections;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator
{
    public class LineIndicator : IndicatorBase
    {
        [SerializeField] private GameObject linePrefab;
        public override void Initialize(Vector3 startPosition, Transform target)
        {
            directionToTarget = (target.position - startPosition).normalized;
            indicator = Instantiate(linePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, directionToTarget)).transform;
            spriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        protected override void SetScale(float progress, float range)
        {
            float scaleY = Mathf.Lerp(0, range, progress);
            float scaleX = indicator.localScale.x;
            indicator.localScale = new Vector3(scaleX, scaleY, 1);
        }
        public override IEnumerator GrowIndicator(Transform target, float duration, float delay, float radius)
        {
            Vector3 initialScale = indicator.localScale;
            initialScale.x = radius;
            indicator.localScale = initialScale;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                directionToTarget = (target.position - transform.position).normalized;
                distanceToTarget = Vector3.Distance(transform.position, target.position);
                SetScale(elapsedTime / duration, distanceToTarget);
                indicator.rotation = Quaternion.LookRotation(Vector3.forward, directionToTarget);
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
    }
}