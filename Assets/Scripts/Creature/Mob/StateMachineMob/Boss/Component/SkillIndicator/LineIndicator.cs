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
        public override void Initialize(Vector3 startPosition, Transform target)
        {
            directionToTarget = (target.position - startPosition).normalized;
            indicator = Instantiate(linePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, directionToTarget)).transform;
            spriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        protected override void SetScale(float progress, float range)
        {
            float scaleY = Mathf.Lerp(0, range, progress);
            indicator.localScale = new Vector3(0.5f, scaleY, 1);
        }
    }
}