using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MidBoss : Boss
{
    public static MidBoss Instance { get; private set; }
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private GameObject vinePrefab;
    [SerializeField] private GameObject meleeAttackPrefab;
    Vector3 meleeAttackPrefabPos;
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
        maxHealthP1 = 100f;
        maxHealthP2 = 200f;
        speed = 1f;
        detectionRange = 10f;
        MeleeAttackRange = 2f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        wall.SetActive(false);
        meleeAttackPrefab.SetActive(false);
        SetupPhase();
    }

    void Update()
    {
        if (curState == State.NONE || curState == State.ATTACK || curState == State.CHANGINGPATTERN)
            return;

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
        Debug.Log("쉬어");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("거리 벌리기");

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange/2)
        {
            yield return StartCoroutine(BackStep(RangedAttackRange));
        }
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern1()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("원거리 공격");

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange)
        {
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
        else
        {
            // 사거리에 도달할 때까지 플레이어 방향으로 이동
            while (distance > RangedAttackRange)
            {
                speed = 10f;
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                distance = Vector3.Distance(transform.position, player.transform.position);
                yield return null;
            }
            // 사거리에 도달한 후 화살 발사 및 대기
            speed = 1f;
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern2()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("근거리 공격");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            meleeAttackPrefab.SetActive(true);
            Debug.Log("켜짐");
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float movement = Vector3.Angle(direction, transform.forward); ;
            meleeAttackPrefab.transform.RotateAround(transform.position, Vector3.forward, movement);
            yield return new WaitForSeconds(1f);
        }
        meleeAttackPrefab.SetActive(false);
        yield return null;
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern3()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(1f);
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
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("히히 씨앗 발사");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange)
        {
            int count = 0;
            while (count < 4)
            {
                Instantiate(seedPrefab, transform.position, Quaternion.identity);
                count++;
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(2f);
        }
        else
        {
            // 사거리에 도달할 때까지 플레이어 방향으로 이동
            while (distance > RangedAttackRange)
            {
                speed = 10f;
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                distance = Vector3.Distance(transform.position, player.transform.position);
                yield return null;
            }
            // 사거리에 도달한 후 화살 발사 및 대기
            speed = 1f;
            int count = 0;
            while (count < 4)
            {
                Instantiate(seedPrefab, transform.position, Quaternion.identity);
                count++;
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(2f);
        }
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern5()
    {
        ChangeState(State.CHANGINGPATTERN);
        Debug.Log("쉬어");
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);
        Debug.Log("덩쿨 휘두르기");
        float duration = 2f;
        float elapsed = 0f;
        Instantiate(vinePrefab, transform.position, Quaternion.identity);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
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
