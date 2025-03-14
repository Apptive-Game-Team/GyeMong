using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Component
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
        public void CreateIndicator(IndicatorType type, Vector3 startPosition, Vector3 direction, float range, float duration)
        {
            GameObject prefab = GetIndicatorPrefab(type);
            IndicatorBase indicator = Instantiate(prefab, startPosition, Quaternion.identity).GetComponent<IndicatorBase>();
            indicator.Initialize(startPosition, direction, range, duration);
            StartCoroutine(indicator.GrowIndicator(range, duration));
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
