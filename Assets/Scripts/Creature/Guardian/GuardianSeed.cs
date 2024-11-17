using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianSeed : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(BombSeed());
    }
    private IEnumerator BombSeed()
    {
        yield return new WaitForSeconds(1f);
        Explode();
        Destroy(gameObject);
    }
    private void Explode()
    {
        float explosionRadius = 2f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Player"))
            {
                PlayerDemo.Instance.Bind(2f);
            }
        }
    }
}
