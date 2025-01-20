using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using Unity.VisualScripting;
using UnityEngine;

public class GrazeController : MonoBehaviour
{
    public static GrazeController Instance { get; set;}
    [SerializeField] private float maxAttackDistance = 1f;
    private List<Collider2D> activeColliders = new();
    private Dictionary<Collider2D, float> colliderDistanceMap = new();
    public Dictionary<Collider2D, bool> colliderAttackedMap = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        for (int i = activeColliders.Count - 1; i >= 0; i--)
        {
            Collider2D collider = activeColliders[i];

            if (collider.gameObject == null)
            {
                if (!colliderAttackedMap[collider])
                {
                    Grazed(collider);
                    RemoveCollider(collider);
                }
                else
                {
                    RemoveCollider(collider);
                }
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("EnemyAttack"))
        {
            if (!activeColliders.Contains(collider))
            {
                activeColliders.Add(collider);
                colliderDistanceMap[collider] = maxAttackDistance;
                colliderAttackedMap[collider] = false;
                Debug.Log($"Collider entered: {collider.name}");
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("EnemyAttack"))
        {
            float curDistance = Vector2.Distance(transform.position, collider.transform.position);

            if (colliderDistanceMap.ContainsKey(collider))
            {
                colliderDistanceMap[collider] = Mathf.Min(curDistance, colliderDistanceMap[collider]);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("EnemyAttack"))
        {
            if (activeColliders.Contains(collider) && !colliderAttackedMap[collider])
            {
                Grazed(collider);
                RemoveCollider(collider);
            }
            else
            {
                RemoveCollider(collider);
            }
        }
    }

    private void Grazed(Collider2D collider)
    {
        if (colliderDistanceMap.TryGetValue(collider, out float distance))
        {
            PlayerCharacter.Instance.GrazeIncreaseGauge(distance);
            Debug.Log($"Gauge Increased by {PlayerCharacter.Instance.gaugeIncreaseValue / distance} with ratio {distance}");
        }
    }

    private void RemoveCollider(Collider2D collider)
    {
        activeColliders.Remove(collider);
        colliderDistanceMap.Remove(collider);
        colliderAttackedMap.Remove(collider);
    }
}
