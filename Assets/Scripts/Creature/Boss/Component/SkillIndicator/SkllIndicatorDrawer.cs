using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Component.SkillIndicator
{
    public class SkllIndicatorDrawer : MonoBehaviour
    {
        public enum IndicatorType
        {
            Line,
            Cone,
            Circle
        }
        [SerializeField] protected GameObject lineIndicatorPrefab;
        [SerializeField] protected GameObject coneIndicatorPrefab;
        [SerializeField] protected GameObject circleIndicatorPrefab;
        public void DrawIndicator(IndicatorType type, Vector3 startPosition, Transform target, float range, float duration)
        {
            GameObject prefab = GetIndicatorPrefab(type);
            IndicatorBase indicator = Instantiate(prefab, startPosition, Quaternion.identity).GetComponent<IndicatorBase>();
            indicator.Initialize(startPosition, target, range, duration);
            StartCoroutine(indicator.GrowIndicator(startPosition, target, range, duration));
        }
        private GameObject GetIndicatorPrefab(IndicatorType type)
        {
            return type switch
            {
                IndicatorType.Line => lineIndicatorPrefab,
                IndicatorType.Cone => coneIndicatorPrefab,
                IndicatorType.Circle => circleIndicatorPrefab,
                _ => null
            };
        }
    }
}
