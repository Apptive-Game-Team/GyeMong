using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Boss : Creature
{
    public GameObject wall;
    private Coroutine detectPlayerRoutine;

    protected int curPattern;
    protected List<int> allPatterns = new List<int> { 0, 1, 2, 3, 4, 5 };
    protected bool isPattern;
    protected int currentPhase = 1;
    protected float maxHealthP1;
    protected float maxHealthP2;
    private int lastPattern = -1; // 직전 패턴을 저장

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
    }
    protected void CheckPhaseTransition()
    {
        if (curHealth <= 0)
        {
            TransPhase();
        }
    }
    protected void TransPhase()
    {
        if (currentPhase < 2)
        {
            currentPhase++;
            SetupPhase();
        }
        else
        {
            Die();
        }
    }

    protected void ExecuteCurrentPattern()
    {
        switch (curPattern)
        {
            case 0:
                StartCoroutine(ExecutePattern0());
                break;
            case 1:
                StartCoroutine(ExecutePattern1());
                break;
            case 2:
                StartCoroutine(ExecutePattern2());
                break;
            case 3:
                StartCoroutine(ExecutePattern3());
                break;
            case 4:
                StartCoroutine(ExecutePattern4());
                break;
            case 5:
                StartCoroutine(ExecutePattern5());
                break;
            default:
                break;
        }
    }

    protected abstract IEnumerator ExecutePattern0();
    protected abstract IEnumerator ExecutePattern1();
    protected abstract IEnumerator ExecutePattern2();
    protected abstract IEnumerator ExecutePattern3();
    protected abstract IEnumerator ExecutePattern4();
    protected abstract IEnumerator ExecutePattern5();

    protected void SelectRandomPattern()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, 3); // 테스트용 범위. 실제로는 Random.Range(0, allPatterns.Count)
        } while (randomIndex == lastPattern); // 직전 패턴과 동일하면 다시 뽑기

        curPattern = allPatterns[randomIndex];
        lastPattern = randomIndex; // 현재 패턴을 직전 패턴으로 저장
    }

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
    public void BackStep()
    {
        if (player != null)
        {
            float backStepSpeed = 10f;
            float step = backStepSpeed * Time.deltaTime;
            Vector3 targetPosition = player.transform.position;
            Vector3 newPosition = Vector3.MoveTowards(transform.position,transform.position - (targetPosition - transform.position), step);
            transform.position = newPosition;
        }
    }

    public IEnumerator DetectPlayerCoroutine()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= detectionRange)
            {
                onBattle = true;
                wall.SetActive(true);
                Debug.Log("qkqh");
                StopDetectingPlayer();
                yield break;
            }
            else onBattle = false;

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
    /*protected override IEnumerator ExecutePattern0()
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
    }*/
}

