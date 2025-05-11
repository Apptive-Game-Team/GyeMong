using UnityEngine;

namespace Gyemong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator
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
    }
}