using System.Collections;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator
{
    public abstract class IndicatorBase : MonoBehaviour
    {
        protected Transform indicator;
        protected SpriteRenderer spriteRenderer;
        protected Vector3 directionToTarget;
        protected float distanceToTarget;
        public abstract void Initialize(Vector3 startPosition, Transform target);
        protected abstract void SetScale(float progress, float range);
        public IEnumerator GrowIndicator(Transform target, float duration, float delay)
        {
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
