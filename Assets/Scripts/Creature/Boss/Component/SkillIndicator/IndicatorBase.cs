using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Component.SkillIndicator
{
    public abstract class IndicatorBase : MonoBehaviour
    {
        protected Transform indicator;
        public abstract void Initialize(Vector3 startPosition, Vector3 direction, float range, float duration);
        public abstract IEnumerator GrowIndicator(float range, float duration);
    }
}
