using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Guardian : Boss
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject cubeShadowPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject meleeAttackPrefab1;
    [SerializeField] private GameObject meleeAttackPrefab2;
    [SerializeField] private GameObject shockwavePrefab;
    private Shield shieldComponenet;
    [SerializeField] private Animator _animator;
    
    void Start()
    {
        curState = State.NONE;
        _fsm = new FiniteStateMachine(new IdleState<Guardian>(this));
        maxPhase = 2;
        maxHealthP1 = 200f;
        maxHealthP2 = 300f;
        shield = 50f;
        shieldComponenet = GetComponent<Shield>();
        speed = 1f;
        detectionRange = 10f;
        MeleeAttackRange = 5f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        wall.SetActive(false);
        meleeAttackPrefab2.GetComponent<MeleeAttack>().attackdamage = defaultDamage;
        meleeAttackPrefab1.GetComponent<MeleeAttack>().attackdamage = defaultDamage;
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

    protected override IEnumerator ExecutePattern0() // Melee Attack
    {
        ChangeState(State.CHANGINGPATTERN);
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            _animator.SetBool("TwoHand", true);
            meleeAttackPrefab1.SetActive(true);
            yield return new WaitForSeconds(1f);
            _animator.SetBool("TwoHand", false);
        }
        meleeAttackPrefab1.SetActive(false);
        yield return null;
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern1() // Falling Cube Attack
    {
        _animator.SetBool("Toss", true);
        ChangeState(State.CHANGINGPATTERN);
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        GameObject cube= Instantiate(cubePrefab, player.transform.position + new Vector3(0, 4, 0), Quaternion.identity);
        GameObject shadow =  Instantiate(cubeShadowPrefab, player.transform.position - new Vector3(0, 0.6f, 0), Quaternion.identity);
        Cube cubeScript = cube.GetComponent<Cube>();
        if (cubeScript != null)
        {
            cubeScript.DetectShadow(shadow);
        }
        yield return null;
        ChangeState(State.IDLE);
        _animator.SetBool("Toss", false);
    }

    protected override IEnumerator ExecutePattern2() // Charge Shield
    {
        ChangeState(State.CHANGINGPATTERN);
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        shield = 30f;
        shieldComponenet.SetActive(true);
        yield return null;
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern3()
    {
        ChangeState(State.CHANGINGPATTERN);
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern4() // Up Stone Attack
    {
        _animator.SetBool("OneHand", true);
        ChangeState(State.CHANGINGPATTERN);
        yield return new WaitForSeconds(1f);
        ChangeState(State.ATTACK);

        int numberOfObjects = 5; // ������ ������Ʈ ��
        float interval = 0.2f; // ���� ����
        float fixedDistance = 7f;

        List<GameObject> spawnedObjects = new List<GameObject>();

        Vector3 direction = (player.transform.position - transform.position).normalized; // �÷��̾� ���� ���
        Vector3 startPosition = transform.position;

        for (int i = 0; i <= numberOfObjects; i++)
        {
            Vector3 spawnPosition = startPosition + direction * (fixedDistance * ((float)i / numberOfObjects));
            GameObject floor = Instantiate(floorPrefab, spawnPosition, Quaternion.identity);
            spawnedObjects.Add(floor);
            yield return new WaitForSeconds(interval); // ���� ������Ʈ �������� ���
        }

        StartCoroutine(DestroyFloor(spawnedObjects, 0.5f));
        ChangeState(State.IDLE);
        _animator.SetBool("OneHand", false);
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
    protected override IEnumerator ExecutePattern5() // Shockwave Attack
    {
        ChangeState(State.CHANGINGPATTERN);
        yield return new WaitForSeconds(2f);
        ChangeState(State.ATTACK);

        _animator.SetBool("LongTwoHand", true);
        yield return new WaitForSeconds(2f);
        yield return MakeShockwave();
        _animator.SetBool("LongTwoHand", false);
        
        yield return null;
        ChangeState(State.IDLE);
    }
    
    private Vector3[] GetCirclePoints(Vector3 center, float radius, int numberOfPoints)
    {
        Vector3[] points = new Vector3[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfPoints;
            float x = center.x + Mathf.Cos(angle) * radius;
            float y = center.y + Mathf.Sin(angle) * radius;
            points[i] = new Vector3(x, y, 0);
        }
        return points;
    }

    private IEnumerator MakeShockwave()
    {
        int startRadius = 4;
        int targetRadius = 14;
        for (int i = startRadius; i <= targetRadius; i++)
        {
            Vector3[] points = GetCirclePoints(transform.position, i, i * 3 + 10);
            for (int j = 0; j < points.Length; j++)
            {
                Instantiate(shockwavePrefab, points[j], Quaternion.identity);
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    
    // protected override IEnumerator ExecutePattern5() // Melee Attack
    // {
    //     ChangeState(State.CHANGINGPATTERN);
    //     yield return new WaitForSeconds(2f);
    //     ChangeState(State.ATTACK);
    //     float distance = Vector3.Distance(transform.position, player.transform.position);
    //     if (distance <= MeleeAttackRange)
    //     {
    //         _animator.SetBool("TwoHand", true);
    //         meleeAttackPrefab2.SetActive(true);
    //         
    //         // �÷��̾� ���� ���
    //         Vector3 playerDirection = (player.transform.position - transform.position).normalized;
    //
    //         // �ݶ��̴��� �÷��̾� �������� �̵�
    //         meleeAttackPrefab2.transform.position = transform.position + playerDirection * MeleeAttackRange;
    //
    //         // ���� ���� �ð�
    //         yield return new WaitForSeconds(1f);
    //
    //         meleeAttackPrefab2.SetActive(false);
    //         _animator.SetBool("TwoHand", false);
    //     }
    //     yield return null;
    //     ChangeState(State.IDLE);
    // }
    protected override void SelectRandomPattern()
    {
        int randomIndex;
        List<int> weightedPatterns = new List<int>();

        // ����ġ�� �ݿ��Ͽ� ����Ʈ�� ������ �߰�
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
        } while (weightedPatterns[randomIndex] == lastPattern); // ���� ���ϰ� �����ϸ� �ٽ� �̱�
        curPattern = weightedPatterns[randomIndex];
        lastPattern = curPattern; // ���� ������ ���� �������� ����
    }
    public void Stun()
    {
        StopAllCoroutines();
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        ChangeState(State.STUN);
        shield = 0f;
        shieldComponenet.SetActive(false);
        yield return new WaitForSeconds(5f);
        ChangeState(State.IDLE);
    }

    protected override void OnShieldBroken()
    {
        base.OnShieldBroken();
        shieldComponenet.SetActive(false);
    }
}
