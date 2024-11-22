using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
public class Guardian : Boss
{
    public static Guardian Instance { get; private set; }

    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject cubeShadowPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject meleeAttackPrefab1;
    [SerializeField] private GameObject meleeAttackPrefab2;
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
        curState = State.NONE;
        _fsm = new FiniteStateMachine(new IdleState(this));
        maxPhase = 2;
        maxHealthP1 = 200f;
        maxHealthP2 = 300f;
        shield = 50f;
        speed = 1f;
        detectionRange = 10f;
        MeleeAttackRange = 2f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        wall.SetActive(false);
        meleeAttackPrefab1.SetActive(false);
        meleeAttackPrefab2.SetActive(false);
        SetupPhase();
    }

    void Update()
    {
        if (curState == State.NONE || curState == State.ATTACK || curState == State.CHANGINGPATTERN)
        {
            return;
        }

        SelectRandomPattern();
        ExecuteCurrentPattern();
        _fsm.UpdateState();
    }
    protected override void Die()
    {
        CheckPhaseTransition();
    }

    protected override IEnumerator ExecutePattern0()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("����");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("�ٰŸ� ����1");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            meleeAttackPrefab1.SetActive(true);
            Debug.Log("����");
            yield return new WaitForSeconds(1f);
        }
        meleeAttackPrefab1.SetActive(false);
        yield return null;
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern1()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("����");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("ť�� ������");
        Instantiate(cubePrefab, player.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
        Instantiate(cubeShadowPrefab, player.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
        yield return null;
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern2()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("����");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("��ȣ��");
        shield = 30f;
        yield return null;
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern3()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("����");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("����");
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            TrackPlayer();
            elapsed += Time.deltaTime;
            yield return null;
        }
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern4()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("����");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("ũ���� q");
        yield return new WaitForSeconds(0.5f);

        int numberOfObjects = 5; // ������ ������Ʈ ��
        float interval = 0.2f; // ���� ����
        float fixedDistance = 7f;

        List<GameObject> spawnedObjects = new List<GameObject>();

        Vector3 direction = (player.transform.position - transform.position).normalized; // �÷��̾� ���� ���
        Vector3 startPosition = transform.position;

        for (int i = 0; i <= numberOfObjects; i++)
        {
            Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
            GameObject floor = Instantiate(floorPrefab, spawnPosition, Quaternion.identity);
            spawnedObjects.Add(floor);
            yield return new WaitForSeconds(interval); // ���� ������Ʈ �������� ���
        }

        StartCoroutine(DestroyFloor(spawnedObjects, 0.5f));
        ChangeState(State.IDLE);
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
    }
    protected override IEnumerator ExecutePattern5()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("����");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("�ٰŸ� ����2");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            meleeAttackPrefab2.SetActive(true);
            Debug.Log("����");
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float movement = Vector3.Angle(direction, transform.forward); ;
            meleeAttackPrefab2.transform.RotateAround(transform.position, Vector3.forward, movement);
            yield return new WaitForSeconds(1f);
        }
        meleeAttackPrefab2.SetActive(false);
        yield return null;
        ChangeState(State.IDLE);
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
            weightedPatterns.AddRange(Enumerable.Repeat(2, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 5));
        }
        else
        {
            weightedPatterns.AddRange(Enumerable.Repeat(0, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(1, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(2, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(4, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(5, 5));
        }
        do
        {
            randomIndex = Random.Range(0, weightedPatterns.Count);
        } while (weightedPatterns[randomIndex] == lastPattern); // ���� ���ϰ� �����ϸ� �ٽ� �̱�
        curPattern = weightedPatterns[randomIndex];
        lastPattern = curPattern; // ���� ������ ���� �������� ����
    }
}
