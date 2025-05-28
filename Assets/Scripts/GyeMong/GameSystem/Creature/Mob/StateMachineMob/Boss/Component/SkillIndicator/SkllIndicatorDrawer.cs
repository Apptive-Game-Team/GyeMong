using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Boss.Component.SkillIndicator
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
        public void DrawIndicator(IndicatorType type, Vector3 startPosition, Transform target, float duration, float delay, float radius = 1f)
        {
            GameObject prefab = GetIndicatorPrefab(type);
            IndicatorBase indicator = Instantiate(prefab, startPosition, Quaternion.identity).GetComponent<IndicatorBase>();
            indicator.Initialize(startPosition, target);
            StartCoroutine(indicator.GrowIndicator(target, duration, delay - 0.1f, radius));
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
