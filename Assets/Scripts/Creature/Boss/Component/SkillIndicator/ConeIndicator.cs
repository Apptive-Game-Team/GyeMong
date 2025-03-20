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
        public override void Initialize(Vector3 startPosition, Transform target)
        {
            directionToTarget = (target.position - startPosition).normalized;
            indicator = Instantiate(conePrefab, startPosition, Quaternion.LookRotation(Vector3.forward, directionToTarget)).transform;
            spriteRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        protected override void SetScale(float progress, float range)
        {
            float scaleY = Mathf.Lerp(0, range, progress);
            indicator.localScale = new Vector3(0.5f, scaleY, 1);
        }
    }
}