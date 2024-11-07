using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidBoss : Boss
{
    private void Awake()
    {
        maxHealthP1 = 100f;
        maxHealthP2 = 200f;
        speed = 1f;
        detectionRange = 10f;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Start()
    {
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

    protected override IEnumerator ExecutePattern1()
    {
        isPattern = true;
        Debug.Log("���Ÿ� ����");
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

    protected override IEnumerator ExecutePattern2()
    {
        isPattern = true;
        Debug.Log("�ٰŸ� ����");
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
