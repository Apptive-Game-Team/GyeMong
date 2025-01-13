using playerCharacter;
using System.Collections;
using UnityEngine;

public class Arrow : GrazeController
{
    private GameObject player;
    private Vector3 direction;
    private float speed = 15f;
    private float attackdamage;
    private SoundObject _soundObject;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _soundObject = GameObject.Find("ArrowHitSoundObject").GetComponent<SoundObject>();
        direction = (player.transform.position - transform.position).normalized;
    }

    private void OnEnable()
    {
        attackdamage = Boss.GetInstance<MidBoss>().defaultDamage;
        StartCoroutine(FireArrow());
    }

    private IEnumerator FireArrow()
    {
        Vector3 firePosition = transform.position;
        float fireDistance = 0;
        while (fireDistance < 10f)
        {
            transform.position += direction * speed * Time.deltaTime;
            fireDistance = Vector3.Distance(firePosition, transform.position);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacked = true;
            _soundObject.PlayAsync();
            Destroy(gameObject);
            PlayerCharacter.Instance.TakeDamage(attackdamage);
        }
    }
}
