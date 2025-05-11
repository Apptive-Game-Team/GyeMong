using UnityEngine;

namespace Gyemong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator
{
    public class ConeIndicator : IndicatorBase
    {
        [SerializeField] private GameObject conePrefab;
        public override void Initialize(Vector3 startPosition, Transform target)
        {
            directionToTarget = (target.position - startPosition).normalized;
            indicator = Instantiate(conePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, directionToTarget)).transform;
            spriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        protected override void SetScale(float progress, float range)
        {
            float scale = Mathf.Lerp(0, range, progress);
            indicator.localScale = new Vector3(scale, scale, 1);
        }
    }
}