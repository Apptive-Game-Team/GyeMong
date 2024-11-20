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
        float accele = 70f; // �߷°��ӵ� (���ӵ� ũ��)
        float speed = 0f; // �ʱ� �ӵ�
        float currentSpeed = speed; // ���� �ӵ�
        Vector3 targetPosition = player.transform.position;
        Vector3 startPosition = transform.position;

        while (transform.position.y > targetPosition.y)
        {
            //�ӵ� = �ʱ�ӵ� + ���ӵ� * �ð�
            currentSpeed += accele * Time.deltaTime;

            //s = vt
            float newY = transform.position.y - currentSpeed * Time.deltaTime;
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
            //�÷��̾ �°� ��� ���� �Ǵ� ����� ���� �ʿ䰡 �־��
        }
    }
}