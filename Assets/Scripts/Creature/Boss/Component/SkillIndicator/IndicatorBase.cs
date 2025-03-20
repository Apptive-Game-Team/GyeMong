using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Component.SkillIndicator
{
    public abstract class IndicatorBase : MonoBehaviour
    {
        [SerializeField] protected GameObject indicatorEffecterPrefab;
        protected float effectSpawnInterval = 0.4f;
        protected Transform indicator;
        protected Vector3 directionToTarget;
        public abstract void Initialize(Vector3 startPosition, Transform target);
        public abstract IEnumerator GrowIndicator(Vector3 startPosition, Transform target, float range, float duration, float delay);
    }
}
