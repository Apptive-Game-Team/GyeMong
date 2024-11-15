using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(OffRootObjects());
    }
    private IEnumerator OffRootObjects()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerDemo.Instance.TakeDamage(10);
        }
    }
}
