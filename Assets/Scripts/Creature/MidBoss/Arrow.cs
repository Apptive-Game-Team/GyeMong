using playerCharacter;
using System.Collections;
using Rework;
using UnityEngine;

public class Arrow : BossAttack
{
    private GameObject player;
    private Vector3 direction;
    private float speed = 15f;
    private SoundObject _soundObject;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _soundObject = GameObject.Find("ArrowHitSoundObject").GetComponent<SoundObject>();
        direction = (player.transform.position - transform.position).normalized;
    }

    private void OnEnable()
    {
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
            _soundObject.PlayAsync();
            Destroy(gameObject);
            PlayerCharacter.Instance.TakeDamage(damage);
        }
    }
}
