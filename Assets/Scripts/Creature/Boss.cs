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
        int randomIndex = Random.Range(0, 2);//�׽�Ʈ�� ������ Random.Range(0, allPatterns.Count);�̰�
        curPattern = allPatterns[randomIndex];
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
    }*/
}

