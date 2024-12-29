using System.Collections;
using System.Collections.Generic;
using System.Linq;
using playerCharacter;
using UnityEngine;

public class MidBoss : Boss
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private GameObject vinePrefab;
    [SerializeField] private GameObject meleeAttackPrefab;
    Vector3 meleeAttackPrefabPos;
    
    private FootSoundController footSoundController;
    [SerializeField] private SoundObject arrowSoundObject;
    [SerializeField] private SoundObject vineSoundObject;

    void Start()
    {
        if (ConditionManager.Instance.Conditions.TryGetValue("spring_midboss_down", out bool down))
        {
            if(down)
            {
                Destroy(gameObject);
            }
        }

        curState = State.NONE;
        _fsm = new FiniteStateMachine(new IdleState<MidBoss>(this));
        maxPhase = 2;
        maxHealthP1 = 100f;
        maxHealthP2 = 200f;
        defaultDamage = 20f;
        speed = 2f;
        shield = 0f;
        detectionRange = 10f;
        MeleeAttackRange = 2f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        wall.SetActive(false);
        meleeAttackPrefab.SetActive(false);
        SetupPhase();
        footSoundController = transform.Find("FootSoundObject").GetComponent<FootSoundController>();
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
     protected override IEnumerator ExecutePattern0() // Back Step
    {
        ChangeState(State.ATTACK);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange/2)
        {
            yield return StartCoroutine(BackStep(RangedAttackRange));
        }
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern1() // Ranged Attack
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance >= RangedAttackRange/2)
        {
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(0.5f);
            ChangeState(State.ATTACK);
            GameObject arrow =  Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            RotateArrowTowardsPlayer(arrow);
            StartCoroutine(arrowSoundObject.Play());
            yield return new WaitForSeconds(1f);
        }
        else // Back Step
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
            GameObject arrow =  Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            RotateArrowTowardsPlayer(arrow);
            StartCoroutine(arrowSoundObject.Play());
            yield return new WaitForSeconds(1f);
        }
        ChangeState(State.IDLE);
    }

    protected override IEnumerator ExecutePattern2() // Melee Attack
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
    protected override IEnumerator ExecutePattern3() // Walk
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            // �ٰŸ� ��Ÿ� ���� �÷��̾ ������ ������ �ǳʶٰ� IDLE ���·� ����
            ChangeState(State.IDLE);
            yield break;
        }
        ChangeState(State.MOVE);
        footSoundController.SetBool(true);
        float duration = 2f;
        float elapsed = 0f;
    
        
        while (elapsed < duration && distance > MeleeAttackRange)
        {
            distance = Vector3.Distance(transform.position, player.transform.position);
            TrackPlayer();
            elapsed += Time.deltaTime;
            yield return null;
        }
        footSoundController.SetBool(false);
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern4() // Seed Ranged Attack
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
                GameObject seed = Instantiate(seedPrefab, transform.position, Quaternion.identity);
                RotateArrowTowardsPlayer(seed);
                StartCoroutine(arrowSoundObject.Play());
                count++;
                yield return new WaitForSeconds(0.25f);
            }
            yield return null;
        }
        else // Back Step
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
                GameObject seed = Instantiate(seedPrefab, transform.position, Quaternion.identity);
                RotateArrowTowardsPlayer(seed);
                StartCoroutine(arrowSoundObject.Play());
                count++;
                yield return new WaitForSeconds(0.25f);
            }
            yield return null;
        }
        ChangeState(State.IDLE);
    }
    protected override IEnumerator ExecutePattern5() // Vine Melee Attack
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            ChangeState(State.CHANGINGPATTERN);
            yield return new WaitForSeconds(0.5f);
            ChangeState(State.ATTACK);
            vineSoundObject.PlayAsync();
            float duration = 2f;
            float elapsed = 0f;
            Instantiate(vinePrefab, transform.position, Quaternion.identity);
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }
            vineSoundObject.Stop();
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
            weightedPatterns.AddRange(Enumerable.Repeat(2, 8));
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
    
    private void RotateArrowTowardsPlayer(GameObject arrow)
    {
        Vector3 direction = (player.transform.position - arrow.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
