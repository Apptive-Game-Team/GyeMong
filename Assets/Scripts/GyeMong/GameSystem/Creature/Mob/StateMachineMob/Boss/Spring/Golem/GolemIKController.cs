using System;
using System.Collections;
using UnityEngine;
using Anima2D;
public class GolemIKController : MonoBehaviour
{
    [Header("IK Targets")]
    [SerializeField] private GameObject ikRight;
    [SerializeField] private GameObject ikRArm;
    [SerializeField] private GameObject ikRHand;
    [SerializeField] private GameObject ikLeft;
    [SerializeField] private GameObject ikLArm;
    [SerializeField] private GameObject ikLHand;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Vector3 rightIdlePos;
    private Vector3 rArmIdlePos;
    private Vector3 rHandIdlePos;
    private Vector3 leftIdlePos;
    private Vector3 lArmIdlePos;
    private Vector3 lHandIdlePos;
    
    private void Awake()
    {
        rightIdlePos = ikRight.transform.position;
        rArmIdlePos  = ikRArm.transform.position;
        rHandIdlePos = ikRHand.transform.position;
        leftIdlePos  = ikLeft.transform.position;
        lArmIdlePos  = ikLArm.transform.position;
        lHandIdlePos = ikLHand.transform.position;
    }

    private void Update()
    {
        // Q → 양손 위아래
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StopAllCoroutines();
            StartCoroutine(HandUpDown());
        }

        // W → 양손 교대로 위아래
        if (Input.GetKeyDown(KeyCode.W))
        {
            StopAllCoroutines();
            StartCoroutine(HandAlternateUpDown());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            StopAllCoroutines();
            StartCoroutine(HandSpread());
        }
    }

    public void MoveIKToPosition(GameObject target, Vector3 targetPos, float duration = 0.5f)
    {
        StartCoroutine(MoveIKCoroutine(target, targetPos, duration));
    }

    private IEnumerator MoveIKCoroutine(GameObject target, Vector3 targetPos, float duration)
    {
        Vector3 startPos = target.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            target.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        target.transform.position = targetPos; //도착 위치 보정
    }

    private Vector3 GetIKPos(GameObject target)
    {
        return target.transform.position;
    }
    public void ResetToIdle()
    {
        MoveIKToPosition(ikRight, rightIdlePos);
        MoveIKToPosition(ikRArm, rArmIdlePos);
        MoveIKToPosition(ikRHand, rHandIdlePos);
        MoveIKToPosition(ikLeft, leftIdlePos);
        MoveIKToPosition(ikLArm, lArmIdlePos);
        MoveIKToPosition(ikLHand, lHandIdlePos);
    }
    public IEnumerator HandUpDown(float duration = 2f, float amplitude = 0.5f, int frequency = 2)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offset = Mathf.Sin(elapsed * frequency * Mathf.PI * 2) * amplitude;
            ikRight.transform.position = rightIdlePos + Vector3.up * offset;
            ikLeft.transform.position = leftIdlePos + Vector3.up * offset;
            yield return null;
        }
        ResetToIdle();
    }
    public IEnumerator HandAlternateUpDown(float duration = 2f, float amplitude = 0.5f, int frequency = 2)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float rOffset = Mathf.Sin(elapsed * frequency * Mathf.PI * 2) * amplitude;
            float lOffset = Mathf.Sin(elapsed * frequency * Mathf.PI * 2 + Mathf.PI) * amplitude;
            ikRight.transform.position = rightIdlePos + Vector3.up * rOffset;
            ikLeft.transform.position = leftIdlePos + Vector3.up * lOffset;
            yield return null;
        }
        ResetToIdle();
    }
    public IEnumerator HandSpread(float duration = 1f, float distance = 1.5f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            ikRight.transform.position = Vector3.Lerp(rightIdlePos, rightIdlePos + Vector3.right * distance, t);
            ikLeft.transform.position  = Vector3.Lerp(leftIdlePos,  leftIdlePos  + Vector3.left  * distance, t);
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        ResetToIdle();
    }
}
