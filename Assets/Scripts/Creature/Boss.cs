using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boss : Creature
{
    public GameObject wall;
    private Coroutine detectPlayerRoutine;

    protected int curPattern;
    protected List<int> allPatterns = new List<int> { 0, 1, 2 };
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

    protected virtual void ExecuteCurrentPattern()
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
            default:
                break;
        }
    }

    protected virtual IEnumerator ExecutePattern0()
    {
        isPattern = true;
        Debug.Log("Executing Pattern 0");
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

    protected virtual IEnumerator ExecutePattern1()
    {
        isPattern = true;
        Debug.Log("Executing Pattern 1");
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

    protected virtual IEnumerator ExecutePattern2()
    {
        isPattern = true;
        Debug.Log("Executing Pattern 2");
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

    protected void SelectRandomPattern()
    {
        int randomIndex = Random.Range(0, allPatterns.Count);
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
}

