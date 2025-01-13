using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

public abstract class GrazeController : MonoBehaviour
{
    private float attackDistance = 1f;
    protected bool isAttacked = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("GrazeCollider"))
        {   
            float curDistance = Mathf.Sqrt(
                Mathf.Pow(transform.position.x - other.transform.position.x, 2) + 
                Mathf.Pow(transform.position.y - other.transform.position.y, 2)
            );
            attackDistance = Mathf.Min(curDistance, attackDistance);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GrazeCollider") && !isAttacked)
        {
            PlayerCharacter.Instance.IncreaseGauge(attackDistance);
            Debug.Log($"Gauge Increased with ratio {attackDistance} and value {10f / attackDistance}");
        }
    }
}
