using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Guardian : Boss
{
    [SerializeField] private GameObject rootPrefab;
    private GameObject[] rootObjects;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject cubeShadowPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject vinePrefab;
    [SerializeField] private List<GameObject> rootSpawnZone;
    private float shieldHealth;
    void Start()
    {
        maxPhase = 2;
        maxHealthP1 = 200f;
        maxHealthP2 = 300f;
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
        //curHealth += shieldHealth;
    }
    protected override void Die()
    {
        CheckPhaseTransition();
    }

    protected override IEnumerator ExecutePattern0()
    {
        isPattern = true;
        Debug.Log("���� �ٴ�");
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
        Debug.Log("ť�� ������");
        Instantiate(cubePrefab, player.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
        Instantiate(cubeShadowPrefab, player.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
        yield return null;
        isPattern = false;
    }

    protected override IEnumerator ExecutePattern2()
    {
        isPattern = true;
        Debug.Log("��ȣ��");
        /*float shield = 30;
        shieldHealth += shield;*/
        yield return null;
        isPattern = false;
    }

    protected override IEnumerator ExecutePattern3()
    {
        isPattern = true;
        Debug.Log("����");
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
        Debug.Log("ũ���� q");
        yield return new WaitForSeconds(0.5f);

        int numberOfObjects = 10; // ������ ������Ʈ ��
        float interval = 0.1f; // ���� ����
        List<GameObject> spawnedObjects = new List<GameObject>();
        for (int i = 0; i <= numberOfObjects; i++)
        {
            Vector3 spawnPosition = Vector3.Lerp(transform.position , player.transform.position, (float)i / numberOfObjects);
            GameObject floor = Instantiate(floorPrefab, spawnPosition, Quaternion.identity);
            spawnedObjects.Add(floor);
            yield return new WaitForSeconds(interval); // ���� ������Ʈ �������� ���
        }

        StartCoroutine(DestroyFloorObjectsAfterDelay(spawnedObjects, 0.5f));

        isPattern = false;
    }
    private IEnumerator DestroyFloorObjectsAfterDelay(List<GameObject> objects, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    } //�̰� ���� �ٴ��� �÷��̾ �����ϸ鼭 ����Ǵµ� �� ��ǥ��ġ�� �����ϰ� ���������� ���°Ŷ� �̰��߿� ���� ������ �𸣰ڴ�

    protected override IEnumerator ExecutePattern5()
    {
        isPattern = true;
        Debug.Log("���� �ֵθ���");
        float duration = 2f;
        float elapsed = 0f;
        Instantiate(vinePrefab, transform.position, Quaternion.identity);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
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

        // ����ġ�� �ݿ��Ͽ� ����Ʈ�� ������ �߰�
        if (currentPhase == 1)
        {
            weightedPatterns.AddRange(Enumerable.Repeat(0, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(1, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(2, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 5));
        }
        else
        {
            weightedPatterns.AddRange(Enumerable.Repeat(0, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(1, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(2, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(4, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(5, 0));
        }
        do
        {
            randomIndex = Random.Range(0, weightedPatterns.Count);
        } while (weightedPatterns[randomIndex] == lastPattern); // ���� ���ϰ� �����ϸ� �ٽ� �̱�
        curPattern = weightedPatterns[randomIndex];
        lastPattern = curPattern; // ���� ������ ���� �������� ����
    }
}