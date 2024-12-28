using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Boss : Creature
{
    private static Dictionary<Type, Boss> _instances = new Dictionary<Type, Boss>();
    
    public static T GetInstance<T>() where T : Boss
    {
        Type type = typeof(T);
        if (_instances.TryGetValue(type, out Boss instance))
        {
            return instance as T;
        }
        return null;
    }
    
    protected virtual void Awake()
    {
        Type type = GetType();
        
        if (_instances.ContainsKey(type) && _instances[type] != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instances[type] = this;
    }

    
    public GameObject wall;
    private Coroutine detectPlayerRoutine;
    protected int currentPhase = 1;
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
    protected int lastPattern = -1;
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        CheckPhaseTransition();
    }
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
                curHealth = 200f;
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
        if (currentPhase < maxPhase)
        {
            currentPhase++;
            StopAllCoroutines();
            StartCoroutine(ChangingPhase());
        }
        else
        {
            Die();
            wall.SetActive(false);
        }
    }
    public IEnumerator ChangingPhase()
    {
        ChangeState(State.CHANGINGPHASE);
        SetupPhase();
        GameObject.Find("PhaseChangeObj").GetComponent<EventObject>().Trigger();
        yield return new WaitForSeconds(2f);
        ChangeState(State.IDLE);
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
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            float backStepSpeed = 50;
            Vector3 direction = (transform.position - playerPosition).normalized;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            LayerMask obstacleLayer = LayerMask.GetMask("Obstacle");
            float currentDistance = Vector3.Distance(transform.position, playerPosition);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);
            int count=0;
            while (hit.collider != null && count<360)
            {
                float angle = UnityEngine.Random.Range(0,360f);
                direction = Quaternion.Euler(0, 0, angle) * direction;
                hit = Physics2D.Raycast(transform.position, direction, targetDistance, obstacleLayer);
                count++;
            }
            if(hit.collider == null)
            {
                currentDistance = targetDistance;
                count = 0;
                float deltaTime = 0.02f;
                while (currentDistance > 0 && count < 100000)
                {
                    Vector3 deltaDistance = direction * backStepSpeed * deltaTime;
                    currentDistance -= deltaDistance.magnitude;
                    Vector3 newPosition = transform.position + deltaDistance;
                    rb.MovePosition(newPosition);
                    count++;
                    yield return new WaitForSeconds(deltaTime);
                }
            }
            else
            {
                yield return null;
            }
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

