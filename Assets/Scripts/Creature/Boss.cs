using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Boss : Creature
{
    public GameObject wall;
    private Coroutine detectPlayerRoutine;
    private Transform playerTransform;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Start()
    {
        wall.SetActive(false);
        playerTransform = player.transform;
    }
    protected override void Die()
    {

    }
    public IEnumerator DetectPlayerCoroutine()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= detectionRange)
            {
                onBattle = true;
                wall.SetActive(true);
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

