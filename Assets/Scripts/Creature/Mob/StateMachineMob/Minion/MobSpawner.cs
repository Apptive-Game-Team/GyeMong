using System.Collections;
using UnityEngine;
using Util.ObjectCreator;

namespace Creature.Mob.StateMachineMob.Minion
{
    public class MobSpawner<T> : MonoBehaviour where T : Mob
    {
        [Header("Spawner Settings")]
    
        [SerializeField] private GameObject slimePrefab;
        private const int MOB_SPAWN_DELAY = 5;
        public int maxSlimeCount = 5;
        public Vector2 spawnAreaSize = new Vector2(5f, 5f);

        private ObjectPool<T> _mobPool;

        void Start()
        {
            _mobPool = new ObjectPool<T>(maxSlimeCount, slimePrefab);

            StartCoroutine(SpawnSlimeRoutine());
        }
    
        private IEnumerator SpawnSlimeRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(MOB_SPAWN_DELAY);


                T mob = _mobPool.GetInactiveObject();
                if (mob != null)
                {
                    SpawnMob(mob);
                }
            }
        }

        private void SpawnMob(T mob)
        {
            Vector3 randomPos = GetRandomPosition();
            mob.transform.position = randomPos;
            mob.gameObject.SetActive(true);

            mob.StartMob();
        }

        private Vector3 GetRandomPosition()
        {
            float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
            float y = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
            return transform.position + new Vector3(x, y, 0f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f));
        }
    }
}
