using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private GameObject player;
    private Vector3 direction;
    private float speed = 10f;
    private float attackdamage;
    
    private SoundObject _soundObject;
    private EventObject _eventObject;
    private void Awake()
    {
        _eventObject = GetComponent<EventObject>();
        _soundObject = GameObject.Find("ArrowHitSoundObject").GetComponent<SoundObject>();
        player = GameObject.FindGameObjectWithTag("Player");
        direction = (player.transform.position - transform.position).normalized;
    }

    private void OnEnable()
    {
        attackdamage = Boss.GetInstance<MidBoss>().defaultDamage;
        StartCoroutine(FireArrow());
    }

    private IEnumerator FireArrow()
    {
        // �÷��̾� ���� ���
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // ȸ�� ���� ���� ���
        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // 45�� ���� �� ������ ���� ����
        float angleRange = 45f;
        float randomAngle = Random.Range(baseAngle - angleRange, baseAngle + angleRange);

        float randomAngleRad = randomAngle * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(randomAngleRad), Mathf.Sin(randomAngleRad), 0).normalized;

        transform.rotation = Quaternion.Euler(0, 0, randomAngle);

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
        // ���� �������� ���ư��� �÷��̾� ��ġ - ���� ��ġ ��ŭ ���ư�
        yield return new WaitForSeconds(1f);
        _soundObject.PlayAsync();
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _soundObject.PlayAsync();
            Destroy(gameObject);
            PlayerCharacter.Instance.TakeDamage(attackdamage);
        }
    }

    private void Explode()
    {
        transform.rotation = Quaternion.identity;
        _eventObject.Trigger();
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
