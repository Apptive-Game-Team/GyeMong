using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using UnityEngine;

public abstract class GrazeController : MonoBehaviour
{
    private float attackDistance = 1f;
    private bool isAttached = false;
    protected bool isAttacked = false;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        isAttached = true;
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("GrazeCollider"))
        {   
            Debug.Log("Attack Distance Calculated");
            float curDistance = Mathf.Sqrt(
                Mathf.Pow(transform.position.x - other.transform.position.x, 2) + 
                Mathf.Pow(transform.position.y - other.transform.position.y, 2)
            );
            attackDistance = Mathf.Min(curDistance, attackDistance);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("GrazeCollider") && !isAttacked)
        {
            isAttached = false;
            PlayerCharacter.Instance.IncreaseGauge(attackDistance);
            Debug.Log($"Gauge Increased with ratio {attackDistance} and value {10f / attackDistance}");
        }
    }

    private void OnDestroy()
    {
        if (isAttached && !isAttacked)
        {
            PlayerCharacter.Instance.IncreaseGauge(attackDistance);
            Debug.Log($"Gauge Increased with ratio {attackDistance} and value {10f / attackDistance}");
        }
    }

    private void OnDisable()
    {
        if (isAttached && !isAttacked)
        {
            PlayerCharacter.Instance.IncreaseGauge(attackDistance);
            Debug.Log($"Gauge Increased with ratio {attackDistance} and value {10f / attackDistance}");
        }
    }
}
