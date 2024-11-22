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
    protected int currentPhase = 2;//�׽�Ʈ�� ������ 1
    protected int maxPhase;
    protected float maxHealthP1;
    protected float maxHealthP2;
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
                wall.SetActive(true);
                Debug.Log("qkqh");
                StopDetectingPlayer();
                ChangeState(State.MOVE);
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

