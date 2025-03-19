using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Component.SkillIndicator
{
    public abstract class IndicatorBase : MonoBehaviour
    {
        protected Transform indicator;
        public Vector3 directionToTarget;
        public abstract void Initialize(Vector3 startPosition, Transform target, float range, float duration);
        public abstract IEnumerator GrowIndicator(Vector3 startPosition, Transform target, float range, float duration);
    }
}
