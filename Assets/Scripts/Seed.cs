using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private GameObject player;
    private Vector3 direction;
    private float speed = 10f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        direction = (player.transform.position - transform.position).normalized;
    }

    private void OnEnable()
    {
        StartCoroutine(FireArrow());
    }

    private IEnumerator FireArrow()
    {
        float randomAngle = Random.Range(0f, 360f);
        direction = new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0).normalized;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = player.transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float distanceMovement = 0f;

        while (distanceMovement < distance)
        {
            transform.position += direction * speed * Time.deltaTime;
            distanceMovement = Vector3.Distance(startPosition, transform.position);
            yield return null;
        }
        //랜덤 방향으로 날아가고 플레이어위치 - 시작 위치 만큼 날아감
        yield return new WaitForSeconds(1f);
        Explode();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerDemo.Instance.TakeDamage(10);
        }
    }

    private void Explode()
    {
        float explosionRadius = 2f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Player"))
            {
                PlayerDemo.Instance.TakeDamage(15);
            }
        }
    }
}
