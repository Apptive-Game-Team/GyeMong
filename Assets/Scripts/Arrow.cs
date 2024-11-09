using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
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
        float timer = 0f;

        while (timer < 5f)
        {
            transform.position += direction * speed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
