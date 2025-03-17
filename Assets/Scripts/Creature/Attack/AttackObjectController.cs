using System.Collections;
using System.Collections.Generic;
using Creature.Attack.Component;
using Creature.Attack.Component.Movement;
using UnityEngine;
using Util.ObjectCreator;

namespace Creature.Attack
{
    public class AttackObjectController : MonoBehaviour
    {
        public bool isGrazed = false;
        public bool isAttacked = false;
        private IAttackObjectMovement _movement;
        [SerializeField] private EnemyAttackInfo _attackInfo;

        private static Dictionary<GameObject, ObjectPool<AttackObjectController>> _objectPools = new();

        public static AttackObjectController Create(Vector3 position, Vector3 direction, GameObject prefab, IAttackObjectMovement movement)
        {
            AttackObjectController attackObjectController = Create(position, direction, prefab);
            attackObjectController._movement = movement;
            return attackObjectController;
        }
        
        public static AttackObjectController Create(Vector3 position, Vector3 direction, GameObject prefab)
        {
            ObjectPool<AttackObjectController> objectPool;
            if (!_objectPools.TryGetValue(prefab, out  objectPool))
            {
                _objectPools[prefab] = new ObjectPool<AttackObjectController>(10, prefab);
            }
            AttackObjectController attackObjectController = objectPool.GetObject();
            attackObjectController.Initialize(position, direction);
            return attackObjectController;
        }
        
        public void StartRoutine()
        {
            StartCoroutine(Routine());
        }
        
        private void Initialize(Vector3 position, Vector3 direction)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        }

        private IEnumerator Routine()
        {
            float elapsedTime = 0;
            while (true)
            {
                Vector3 position = _movement.GetPosition(elapsedTime);
                transform.position = position;
                yield return null;
            }
        }
    }
}