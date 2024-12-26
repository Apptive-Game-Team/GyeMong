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
        speed = 2f;
        detectionRange = 10f;
        MeleeAttackRange = 2f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        wall.SetActive(false);
        meleeAttackPrefab.SetActive(false);
        SetupPhase();
    }

    protected override void Update()
    {
        base.Update();
        if(curState == State.IDLE && currentPatternCoroutine == null)
        {
            SelectRandomPattern();
            ExecuteCurrentPattern();
        }
        _fsm.UpdateState();
    }
    protected override IEnumerator ExecutePattern0()//백스텝
    {
        ChangeState(State.ATTACK);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange/2)
        {
            yield return StartCoroutine(BackStep(RangedAttackRange));
        }
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern1()//원거리 활
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance >= RangedAttackRange/2)
        {
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(0.5f);
            ChangeState(State.ATTACK);
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
        else
        {
            dashType = 1;
            ChangeState(State.DASH);
            distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < RangedAttackRange)
            {
                yield return StartCoroutine(BackStep(RangedAttackRange));
            }
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(0.5f);
            ChangeState(State.ATTACK);
            speed = 1f;
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(1f);
        }
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern2()//근거리
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(0.2f);
            ChangeState(State.ATTACK);
            meleeAttackPrefab.SetActive(true);

            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            meleeAttackPrefab.transform.position = transform.position + playerDirection * MeleeAttackRange;
            yield return new WaitForSeconds(0.3f);

            meleeAttackPrefab.SetActive(false);
        }
        yield return null;
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern3()//추적
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            // 근거리 사거리 내에 플레이어가 있으면 패턴을 건너뛰고 IDLE 상태로 변경
            ChangeState(State.IDLE);
            yield break;
        }
        ChangeState(State.MOVE);
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
    protected override IEnumerator ExecutePattern4()//원거리 씨앗
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance >= RangedAttackRange/2)
        {
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(1f);
            ChangeState(State.ATTACK);
            int count = 0;
            while (count < 4)
            {
                Instantiate(seedPrefab, transform.position, Quaternion.identity);
                count++;
                yield return new WaitForSeconds(0.25f);
            }
            yield return null;
        }
        else
        {
            dashType = 1;
            ChangeState(State.DASH);
            distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < RangedAttackRange)
            {
                yield return StartCoroutine(BackStep(RangedAttackRange));
            }
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(1f);
            ChangeState(State.ATTACK);
            speed = 1f;
            int count = 0;
            while (count < 4)
            {
                Instantiate(seedPrefab, transform.position, Quaternion.identity);
                count++;
                yield return new WaitForSeconds(0.25f);
            }
            yield return null;
        }
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern5()//덩쿨 휘두르기
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(0.5f);
            ChangeState(State.ATTACK);
            float duration = 2f;
            float elapsed = 0f;
            Instantiate(vinePrefab, transform.position, Quaternion.identity);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        yield return null;
        ChangeState(State.IDLE);
    }

    protected override void SelectRandomPattern()
    {
        int randomIndex;
        List<int> weightedPatterns = new List<int>();

        if (currentPhase == 1)
        {
            weightedPatterns.AddRange(Enumerable.Repeat(0, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(1, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(2, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 5));
        }
        else
        {
            weightedPatterns.AddRange(Enumerable.Repeat(0, 0));
            weightedPatterns.AddRange(Enumerable.Repeat(1, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(2, 12));
            weightedPatterns.AddRange(Enumerable.Repeat(3, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(4, 5));
            weightedPatterns.AddRange(Enumerable.Repeat(5, 8));
        }
        do
        {
            randomIndex = Random.Range(0, weightedPatterns.Count);
        } while (weightedPatterns[randomIndex] == lastPattern);
        curPattern = weightedPatterns[randomIndex];
        lastPattern = curPattern;
    }
}
