using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class Boss : Creature
{
    public GameObject wall;
    private Coroutine detectPlayerRoutine;

    protected int curPattern;
    protected bool isPattern;
    protected int currentPhase = 1;//�׽�Ʈ�� ������ 1
    protected float maxHealthP1;
    protected float maxHealthP2;
    private int lastPattern = -1; // ���� ������ ����

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
        List<int> weightedPatterns = new List<int>();

        // ����ġ�� �ݿ��Ͽ� ����Ʈ�� ������ �߰�
        if(currentPhase == 1)
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
        } while (weightedPatterns[randomIndex] == lastPattern); // ���� ���ϰ� �����ϸ� �ٽ� �̱�
        curPattern = weightedPatterns[randomIndex];
        lastPattern = curPattern; // ���� ������ ���� �������� ����
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
    /*public void BackStep()
    {
        if (player != null)
        {
            float backStepSpeed = 5f;
            float step = backStepSpeed * Time.deltaTime;
            Vector3 targetPosition = player.transform.position;
            Vector3 newPosition = Vector3.MoveTowards(transform.position,transform.position - (targetPosition - transform.position), step);
            transform.position = newPosition;
        }
    }*/
    public IEnumerator BackStep(float duration)
    {
        if (player != null)
        {
            float elapsedTime = 0f;
            float backStepSpeed = 2f;
            Vector3 direction = (transform.position - player.transform.position).normalized; // �÷��̾� �ݴ� ����

            while (elapsedTime < duration)
            {
                // �� �����Ӹ��� �̵�
                transform.position += direction * backStepSpeed * Time.deltaTime;
                elapsedTime += Time.deltaTime;
            }
            yield return new WaitForSeconds(1f);
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

