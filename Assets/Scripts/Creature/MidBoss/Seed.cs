using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private GameObject player;
    private Vector3 direction;
    private float speed = 10f;
    private float attackdamage = MidBoss.GetInstance<MidBoss>().defaultDamage;
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
        // 플레이어 방향 계산
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // 회전 기준 각도 계산
        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // 45도 범위 내 무작위 각도 생성
        float angleRange = 45f;
        float randomAngle = Random.Range(baseAngle - angleRange, baseAngle + angleRange);

        float randomAngleRad = randomAngle * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(randomAngleRad), Mathf.Sin(randomAngleRad), 0).normalized;

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
        // 랜덤 방향으로 날아가고 플레이어 위치 - 시작 위치 만큼 날아감
        yield return new WaitForSeconds(1f);
        Explode();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerCharacter.Instance.TakeDamage(attackdamage);
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
                PlayerCharacter.Instance.TakeDamage(attackdamage/2);
            }
        }
    }
}
