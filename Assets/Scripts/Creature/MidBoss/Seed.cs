using playerCharacter;
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
        //���� �������� ���ư��� �÷��̾���ġ - ���� ��ġ ��ŭ ���ư�
        yield return new WaitForSeconds(1f);
        Explode();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerCharacter.Instance.TakeDamage(10);
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
                PlayerCharacter.Instance.TakeDamage(15);
            }
        }
    }
}
