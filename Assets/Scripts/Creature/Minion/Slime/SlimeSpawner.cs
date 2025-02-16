using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject slimePrefab;
    public int maxSlimeCount = 5;
    public Vector2 spawnAreaSize = new Vector2(5f, 5f);

    private List<GameObject> slimePool;

    void Start()
    {
        slimePool = new List<GameObject>();

        for (int i = 0; i < maxSlimeCount; i++)
        {
            GameObject slime = Instantiate(slimePrefab);
            slime.SetActive(false);
            slimePool.Add(slime);
        }

        SpawnSlimes();
    }

    void Update()
    {
        foreach (GameObject slime in slimePool)
        {
            if (!slime.activeInHierarchy)
            {
                SpawnSlime(slime);
            }
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

    private void SpawnSlimes()
    {
        foreach (GameObject slime in slimePool)
        {
            SpawnSlime(slime);
        }
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
