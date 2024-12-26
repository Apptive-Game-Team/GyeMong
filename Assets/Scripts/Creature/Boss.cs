using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Boss : Creature
{
    public GameObject wall;
    private Coroutine detectPlayerRoutine;
    protected int currentPhase = 2; //�׽�Ʈ�� ������ 1
    protected int maxPhase;
    protected float maxHealthP1;
    protected float maxHealthP2;

    public float CurrentHp
    {
        get { return curHealth; }
    }

    public int CurrentPhase
    {
        get { return currentPhase; }
    }
    public Tuple<int, float, float> BossInfo
    {
        get { return new Tuple<int, float, float>(currentPhase, maxHealthP1, maxHealthP2); }
    }
    protected int lastPattern = -1; // ���� ������ ����

    protected void SetupPhase()
    {
        switch (currentPhase)
        {
            case 1:
                curHealth = maxHealthP1;
                break;
            case 2:
                curHealth = maxHealthP2;
                break;

            default:
                break;
        }
    } //�ִ������� �߰��� case�߰�
    protected void CheckPhaseTransition()
    {
        if (curHealth <= 0)
        {
            TransPhase();
        }
    }
    protected void TransPhase()
    {
        if (currentPhase < maxPhase)
        {
            currentPhase++;
            SetupPhase();
        }
        else
        {
            Die();
        }
    }
    public Coroutine currentPatternCoroutine;
    protected void ExecuteCurrentPattern()
    {
        if (currentPatternCoroutine != null) return; // �ߺ� ���� ����
        switch (curPattern)
        {
            case 0:
                currentPatternCoroutine = StartCoroutine(ExecutePattern0());
                break;
            case 1:
                currentPatternCoroutine = StartCoroutine(ExecutePattern1());
                break;
            case 2:
                currentPatternCoroutine = StartCoroutine(ExecutePattern2());
                break;
            case 3:
                currentPatternCoroutine = StartCoroutine(ExecutePattern3());
                break;
            case 4:
                currentPatternCoroutine = StartCoroutine(ExecutePattern4());
                break;
            case 5:
                currentPatternCoroutine = StartCoroutine(ExecutePattern5());
                break;
        }
    } //���� ���� �߰��� case�߰�
    protected abstract IEnumerator ExecutePattern0();
    protected abstract IEnumerator ExecutePattern1();
    protected abstract IEnumerator ExecutePattern2();
    protected abstract IEnumerator ExecutePattern3();
    protected abstract IEnumerator ExecutePattern4();
    protected abstract IEnumerator ExecutePattern5();
    protected abstract void SelectRandomPattern();//���� ���� �ż��� �߻�ȭ
    public void TrackPlayer()
    {
        if (player != null)
        {
            float step = speed * Time.deltaTime;
            Vector3 targetPosition = player.transform.position;
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);
            transform.position = newPosition;
        }
    }
    public IEnumerator BackStep(float targetDistance)
    {
        if (player == null) 
            yield break;

        float backStepSpeed = 50f;
        float checkRadius = 1f;
        LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector3 direction = GetSafeDirection(targetDistance, checkRadius, obstacleLayer);

        // 안전한 방향이 없으면 백스텝 중단
        if (direction == Vector3.zero) yield break;

        // 백스텝 이동
        while (Vector3.Distance(transform.position, player.transform.position) < targetDistance)
        {
            if (IsObstacleInPath(transform.position, direction, checkRadius, obstacleLayer, backStepSpeed))
                yield break;

            Vector3 newPosition = transform.position + direction * backStepSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }

    // 안전한 방향 찾기
    private Vector3 GetSafeDirection(float distance, float radius, LayerMask layer)
    {
        Vector3 initialDirection = (transform.position - player.transform.position).normalized;
        int numDirections = 8;
        List<Vector3> validDirections = new List<Vector3>();

        for (int i = 0; i < numDirections; i++)
        {
            float angle = i * (360f / numDirections);
            Vector3 testDirection = Quaternion.Euler(0, 0, angle) * initialDirection;

            if (!Physics2D.CircleCast(transform.position, radius, testDirection, distance, layer))
            {
                validDirections.Add(testDirection.normalized);
            }
        }

        return validDirections.Count > 0
            ? validDirections[UnityEngine.Random.Range(0, validDirections.Count)]
            : Vector3.zero; // 모든 방향이 막혀 있으면 Vector3.zero 반환
    }

    // 이동 경로 충돌 검사
    private bool IsObstacleInPath(Vector3 position, Vector3 direction, float radius, LayerMask layer, float speed)
    {
        return Physics2D.CircleCast(position, radius, direction, speed * Time.deltaTime, layer);
    }

    public IEnumerator DetectPlayerCoroutine()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= detectionRange)
            {
                wall.SetActive(true);
                StopDetectingPlayer();
                ChangeState(State.IDLE);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void StartDetectingPlayer()
    {
        if (detectPlayerRoutine == null)
        {
            detectPlayerRoutine = StartCoroutine(DetectPlayerCoroutine());
        }
    }

    public void StopDetectingPlayer()
    {
        if (detectPlayerRoutine != null)
        {
            StopCoroutine(detectPlayerRoutine);
            detectPlayerRoutine = null;
        }
    }
}

