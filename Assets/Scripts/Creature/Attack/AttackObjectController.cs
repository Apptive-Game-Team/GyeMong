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

        public EnemyAttackInfo AttackInfo => _attackInfo;
        
        private static Dictionary<GameObject, ObjectPool<AttackObjectController>> _objectPools = new();

        public static AttackObjectController Create(Vector3 position, Vector3 direction, GameObject prefab, IAttackObjectMovement movement)
        {
            AttackObjectController attackObjectController = Create(position, direction, prefab);
            attackObjectController._movement = movement;
            return attackObjectController;
        }
        
        public static AttackObjectController Create(Vector3 position, Vector3 direction, GameObject prefab, float duration)
        {
            AttackObjectController attackObjectController = Create(position, direction, prefab);
            attackObjectController._movement = new StaticMovement(position, duration);
            return attackObjectController;
        }
        
        private static AttackObjectController Create(Vector3 position, Vector3 direction, GameObject prefab)
        {
            ObjectPool<AttackObjectController> objectPool;
            if (!_objectPools.TryGetValue(prefab, out objectPool))
            {
                _objectPools[prefab] = new ObjectPool<AttackObjectController>(1, prefab);
                objectPool = _objectPools[prefab];
            }
            AttackObjectController attackObjectController = objectPool.GetObject();
            attackObjectController.gameObject.SetActive(true);
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
                elapsedTime += Time.deltaTime;
                Vector3? position = _movement.GetPosition(elapsedTime);
                if (!position.HasValue)
                {
                    gameObject.SetActive(false);
                    break;
                }
                transform.position = position.Value;
                yield return null;
            }
        }
    }
}