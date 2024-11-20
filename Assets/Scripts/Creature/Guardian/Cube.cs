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
        float accele = 70f; // 중력가속도 (가속도 크기)
        float speed = 0f; // 초기 속도
        float currentSpeed = speed; // 현재 속도
        Vector3 targetPosition = player.transform.position;
        Vector3 startPosition = transform.position;

        while (transform.position.y > targetPosition.y)
        {
            //속도 = 초기속도 + 가속도 * 시간
            currentSpeed += accele * Time.deltaTime;

            //s = vt
            float newY = transform.position.y - currentSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            yield return null;
        }

        // 낙하 완료 후 Collider의 isTrigger를 해제
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
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