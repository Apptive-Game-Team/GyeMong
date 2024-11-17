using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Cube : MonoBehaviour
{
    private GameObject player;

    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FollowAndFall());
    }

    private IEnumerator FollowAndFall()
    {
        float followDuration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < followDuration)
        {
            if (player != null)
            {
                transform.position = player.transform.position + new Vector3 (0,4,0);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(StartFalling());
    }

    private IEnumerator StartFalling()
    {
        float Speed = 7f; // 낙하 속도
        Vector3 targetPosition = player.transform.position;
        Vector3 startPosition = transform.position;

        float distance = startPosition.y - targetPosition.y;
        float elapsedTime = 0f;

        while (transform.position.y > targetPosition.y)
        {
            elapsedTime += Time.deltaTime;
            float newY = Mathf.Lerp(startPosition.y, targetPosition.y, elapsedTime * Speed / distance);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerDemo.Instance.TakeDamage(10);
            //플레이어가 맞고 잠시 무적 되는 기능을 넣을 필요가 있어보임
        }
    }
}