using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Guardian : Boss
{
    public static Guardian Instance { get; private set; }

    [SerializeField] private GameObject rootPrefab;
    private GameObject[] rootObjects;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject cubeShadowPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private List<GameObject> rootSpawnZone;
    private float shieldHealth;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        maxPhase = 2;
        maxHealthP1 = 200f;
        maxHealthP2 = 300f;
        shield = 50f;
        speed = 1f;
        detectionRange = 10f;
        MeleeAttackRange = 1f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        wall.SetActive(false);
        SetupPhase();

        rootObjects = new GameObject[rootSpawnZone.Count];
        for (int i = 0; i < rootSpawnZone.Count; i++)
        {
            GameObject rootObject = Instantiate(rootPrefab, rootSpawnZone[i].transform.position , Quaternion.identity);
            rootObjects[i] = rootObject;
            rootObject.SetActive(false);
        }
    }

    void Update()
    {
        if (onBattle)
        {
            if(!isPattern)
            {
                SelectRandomPattern();
                ExecuteCurrentPattern();
            }
        }
    }
    protected override void Die()
    {
        CheckPhaseTransition();
    }

    protected override IEnumerator ExecutePattern0()
    {
        isPattern = true;
        Debug.Log("가시 바닥");
        int caseNumber = Random.Range(1, 5);
        if(!rootObjects[0].activeSelf && !rootObjects[3].activeSelf)
        {
            switch (caseNumber)
            {
                case 1:
                    ActivateRootObjects(new int[] { 0, 1, 7, 8, 9, 10, 11 });
                    break;
                case 2:
                    ActivateRootObjects(new int[] { 1, 2, 3, 5, 9, 11, 12, 13 });
                    break;
                case 3:
                    ActivateRootObjects(new int[] { 0, 2, 4, 6, 8, 10, 12, 14 });
                    break;
                case 4:
                    ActivateRootObjects(new int[] { 0, 1, 3, 6, 8, 9, 11, 12, 14 });
                    break;
            }
            yield return null;
        }
        isPattern = false;
    }
    protected override IEnumerator ExecutePattern1()
    {
        isPattern = true;
        Debug.Log("큐브 떨구기");
        Instantiate(cubePrefab, player.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
        Instantiate(cubeShadowPrefab, player.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
        yield return null;
        isPattern = false;
    }

    protected override IEnumerator ExecutePattern2()
    {
        isPattern = true;
        Debug.Log("보호막");
        shield = 30f;
        yield return null;
        isPattern = false;
    }

    protected override IEnumerator ExecutePattern3()
    {
        isPattern = true;
        Debug.Log("추적");
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            TrackPlayer();
            elapsed += Time.deltaTime;
            yield return null;
        }
        isPattern = false;
    }
    protected override IEnumerator ExecutePattern4()
    {
        isPattern = true;
        Debug.Log("크산테 q");
        yield return new WaitForSeconds(0.5f);

        int numberOfObjects = 5; // 생성할 오브젝트 수
        float interval = 0.2f; // 생성 간격
        float fixedDistance = 7f;

        List<GameObject> spawnedObjects = new List<GameObject>();

        Vector3 direction = (player.transform.position - transform.position).normalized; // 플레이어 방향 계산
        Vector3 startPosition = transform.position;

        for (int i = 0; i <= numberOfObjects; i++)
        {
            Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
            GameObject floor = Instantiate(floorPrefab, spawnPosition, Quaternion.identity);
            spawnedObjects.Add(floor);
            yield return new WaitForSeconds(interval); // 다음 오브젝트 생성까지 대기
        }

        StartCoroutine(DestroyFloor(spawnedObjects, 0.5f));

        isPattern = false;
    }
    private IEnumerator DestroyFloor(List<GameObject> objects, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    } //이게 지금 바닥이 플레이어를 추적하면서 융기되는데 걍 목표위치를 고정하고 직선형으로 뻗는거랑 이거중에 뭐가 나을지 모르겠다

    protected override IEnumerator ExecutePattern5()
    {
        isPattern = true;
        Debug.Log("맞으면 못 움직이는 씨앗");
        Instantiate(seedPrefab,player.transform.position, Quaternion.identity);
        yield return null;
        isPattern = false;
    }
    private void ActivateRootObjects(int[] indices)
    {
        foreach (int index in indices)
        {
           rootObjects[index].SetActive(true);
        }
    }
    protected override void SelectRandomPattern()
    {
        int randomIndex;
        List<int> weightedPatterns = new List<int>();

        // 가중치를 반영하여 리스트에 패턴을 추가
        if (currentPhase == 1)
        {
            weightedPatterns.AddRange(Enumerable.Repeat(0, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(1, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(2, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 5));
        }
        else
        {
            weightedPatterns.AddRange(Enumerable.Repeat(0, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(1, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(2, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(4, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(5, 5));
        }
        do
        {
            randomIndex = Random.Range(0, weightedPatterns.Count);
        } while (weightedPatterns[randomIndex] == lastPattern); // 직전 패턴과 동일하면 다시 뽑기
        curPattern = weightedPatterns[randomIndex];
        lastPattern = curPattern; // 현재 패턴을 직전 패턴으로 저장
    }
}
