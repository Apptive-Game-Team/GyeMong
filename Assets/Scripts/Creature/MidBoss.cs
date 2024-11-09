using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MidBoss : Boss
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private GameObject vinePrefab;
    void Start()
    {
        maxHealthP1 = 100f;
        maxHealthP2 = 200f;
        speed = 1f;
        detectionRange = 10f;
        MeleeAttackRange = 1f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        //여기까지 원래 Awake였음
        wall.SetActive(false);
        SetupPhase();
    }

    void Update()
    {
        if (onBattle)
        {
            if(!isPattern)
            {
                SelectRandomPattern();
                ExecuteCurrentPattern();
            }
        }
    }
    protected override void Die()
    {
        CheckPhaseTransition();
    }

    protected override IEnumerator ExecutePattern0()
    {
        isPattern = true;
        Debug.Log("거리 벌리기");

        float duration = 1f; // 이동 시간
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange)
        {
            yield return StartCoroutine(BackStep(duration));
        }
        isPattern = false;
    }
    protected override IEnumerator ExecutePattern1()
    {
        isPattern = true;
        Debug.Log("원거리 공격");

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange)
        {
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
        else
        {
            // 사거리에 도달할 때까지 플레이어 방향으로 이동
            while (distance > RangedAttackRange)
            {
                speed = 10f;
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                distance = Vector3.Distance(transform.position, player.transform.position);
                yield return null;
            }
            // 사거리에 도달한 후 화살 발사 및 대기
            speed = 1f;
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }

        isPattern = false;
    }

    protected override IEnumerator ExecutePattern2()
    {
        isPattern = true;
        Debug.Log("근거리 공격");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= MeleeAttackRange)
        {
            PlayerDemo.Instance.TakeDamage(10);
        }
        isPattern = false;
        yield return null;
    }

    protected override IEnumerator ExecutePattern3()
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
    }
    protected override IEnumerator ExecutePattern4()
    {
        isPattern = true;
        Debug.Log("히히 씨앗 발사");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange)
        {
            int count = 0;
            while(count < 4)
            {
                Instantiate(seedPrefab, transform.position, Quaternion.identity);
                count++;
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(2f);
        }
        else
        {
            // 사거리에 도달할 때까지 플레이어 방향으로 이동
            while (distance > RangedAttackRange)
            {
                speed = 10f;
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                distance = Vector3.Distance(transform.position, player.transform.position);
                yield return null;
            }
            // 사거리에 도달한 후 화살 발사 및 대기
            speed = 1f;
            yield return new WaitForSeconds(0.5f);
            int count = 0;
            while (count < 4)
            {
                Instantiate(seedPrefab, transform.position, Quaternion.identity);
                count++;
                yield return new WaitForSeconds(0.3f);
            }
            yield return new WaitForSeconds(2f);
        }

        isPattern = false;
    }
    protected override IEnumerator ExecutePattern5()
    {
        isPattern = true;
        Debug.Log("덩쿨 휘두르기");
        float duration = 2f;
        float elapsed = 0f;
        Instantiate(vinePrefab, transform.position, Quaternion.identity);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        isPattern = false;
    }
}
