using System.Collections;
using UnityEngine;

public class Vine : MonoBehaviour
{
    private bool hasDamaged = false; // �������� �� ���� �ֱ� ���� �÷���
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
            PlayerDemo.Instance.TakeDamage(15);
            hasDamaged = true;  // �������� �� ���� �ֵ��� ����
        }
    }
}
