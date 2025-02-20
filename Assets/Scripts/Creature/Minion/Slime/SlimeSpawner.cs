using System.Collections;
using UnityEngine;
using Util.ObjectCreator;

public class SlimeSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    
    [SerializeField] private GameObject slimePrefab;
    private const int SLIME_SPAWN_DELAY = 5;
    public int maxSlimeCount = 5;
    public Vector2 spawnAreaSize = new Vector2(5f, 5f);

    private ObjectPool _slimePool;

    void Start()
    {
        _slimePool = new ObjectPool(maxSlimeCount, slimePrefab);

        StartCoroutine(SpawnSlimeRoutine());
    }
    
    private IEnumerator SpawnSlimeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(SLIME_SPAWN_DELAY);
            GameObject slime = _slimePool.GetObject();
            SpawnSlime(slime);
        }
    }

    private void SpawnSlime(GameObject slime)
    {
        Vector3 randomPos = GetRandomPosition();
        slime.transform.position = randomPos;
        slime.SetActive(true);

        Slime slimeScript = slime.GetComponent<Slime>();
        slimeScript.HealSlime();
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
