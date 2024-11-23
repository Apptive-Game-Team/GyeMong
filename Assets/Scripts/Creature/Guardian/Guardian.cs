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
        if(curState == State.IDLE)
        {
            SelectRandomPattern();
            ExecuteCurrentPattern();
        }
        _fsm.UpdateState();
    }
    protected override void Die()
    {
        CheckPhaseTransition();
    }

    protected override IEnumerator ExecutePattern0()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        Debug.Log("근거리 공격1");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            meleeAttackPrefab1.SetActive(true);
            Debug.Log("켜짐");
            yield return new WaitForSeconds(1f);
        }
        meleeAttackPrefab1.SetActive(false);
        yield return null;
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern1()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        Debug.Log("큐브 떨구기");
        Instantiate(cubePrefab, player.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
        Instantiate(cubeShadowPrefab, player.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
        yield return null;
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern2()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        Debug.Log("보호막");
        shield = 30f;
        yield return null;
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern3()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        Debug.Log("추적");
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
        Debug.Log("쉬어");
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
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
        Debug.Log("쉬어");
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        Debug.Log("근거리 공격2");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            meleeAttackPrefab2.SetActive(true);
            Debug.Log("켜짐");
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
            weightedPatterns.AddRange(Enumerable.Repeat(2, 5));
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
