using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MidBoss : Boss
{
    [SerializeField] private GameObject arrowPrefab;
    void Start()
    {
        maxHealthP1 = 100f;
        maxHealthP2 = 200f;
        speed = 1f;
        detectionRange = 10f;
        MeleeAttackRange = 1f;
        RangedAttackRange = 6f;
        player = GameObject.FindGameObjectWithTag("Player");
        //������� ���� Awake����
        wall.SetActive(false);
        SelectRandomPattern();
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
        Debug.Log("�Ÿ� ������");

        float duration = 1f; // �̵� �ð�
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
        Debug.Log("���Ÿ� ����");

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= RangedAttackRange)
        {
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
        else
        {
            // ��Ÿ��� ������ ������ �÷��̾� �������� �̵�
            while (distance > RangedAttackRange)
            {
                speed = 10f;
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
                distance = Vector3.Distance(transform.position, player.transform.position);
                yield return null;
            }
            // ��Ÿ��� ������ �� ȭ�� �߻� �� ���
            speed = 1f;
            Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }

        isPattern = false;
    }

    protected override IEnumerator ExecutePattern2()
    {
        isPattern = true;
        Debug.Log("�ٰŸ� ����");
        float distance = Vector3.Distance(transform.position, player.transform.position);
        while (distance <= RangedAttackRange)
        {
            PlayerDemo.Instance.TakeDamage(10);
            break;
        }
        isPattern = false;
        yield return null;
    }

    protected override IEnumerator ExecutePattern3()
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
    }
    protected override IEnumerator ExecutePattern4()
    {
        isPattern = true;
        Debug.Log("���� ���� �߻�");
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
    protected override IEnumerator ExecutePattern5()
    {
        isPattern = true;
        Debug.Log("���� �ֵθ���");
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
}
