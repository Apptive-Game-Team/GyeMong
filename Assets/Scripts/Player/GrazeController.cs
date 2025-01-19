using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using Unity.VisualScripting;
using UnityEngine;

public class GrazeController : MonoBehaviour
{
    [SerializeField] private float maxAttackDistance = 1f;
    private List<Collider2D> activeColliders = new();
    private Dictionary<Collider2D, float> distanceMap = new();
    public Dictionary<Collider2D, bool> attackedMap = new();

    private void Update()
    {
        foreach (var collider in activeColliders)
        {
            if (!attackedMap[collider])
            {
                if (!collider.gameObject.activeSelf || collider.gameObject.IsDestroyed())
                {
                    Grazed(collider);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            if (!activeColliders.Contains(other))
            {
                activeColliders.Add(other);
                distanceMap[other] = maxAttackDistance;
                attackedMap[other] = false;
                Debug.Log($"Collider entered: {other.name}");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            float curDistance = Vector2.Distance(transform.position, other.transform.position);

            if (distanceMap.ContainsKey(other))
            {
                distanceMap[other] = Mathf.Min(curDistance, distanceMap[other]);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            if (activeColliders.Contains(other))
            {
                Grazed(other);
            }
        }
    }

    private void Grazed(Collider2D collider)
    {
        if (distanceMap.TryGetValue(collider, out float distance))
        {
            PlayerCharacter.Instance.GrazeIncreaseGauge(distance);
            Debug.Log($"Gauge Increased by {PlayerCharacter.Instance.gaugeIncreaseValue / distance} with ratio {distance}");
            activeColliders.Remove(collider);
            distanceMap.Remove(collider);
        }
    }
}
