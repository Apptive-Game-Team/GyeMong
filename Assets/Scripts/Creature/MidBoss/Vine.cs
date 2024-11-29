using playerCharacter;
using System.Collections;
using UnityEngine;

public class Vine : MonoBehaviour
{
    private bool hasDamaged = false; // 데미지를 한 번만 주기 위한 플래그
    private void OnEnable()
    {
        StartCoroutine(ActivateVine());
    }
    private IEnumerator ActivateVine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasDamaged && other.CompareTag("Player"))
        {
            PlayerCharacter.Instance.TakeDamage(15);
            hasDamaged = true;  // 데미지를 한 번만 주도록 설정
        }
    }
}
