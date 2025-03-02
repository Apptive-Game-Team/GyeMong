using System;
using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using Unity.VisualScripting;
using UnityEngine;
using Creature.Boss;
public class GrazeController : MonoBehaviour
{
    public static GrazeController Instance { get; set;}
    [SerializeField] private float maxAttackDistance = 1f;
    private List<Collider2D> activeColliders = new();
    private Dictionary<Collider2D, float> colliderDistanceMap = new();
    private PlayerSoundController _playerSoundController;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        _playerSoundController = transform.parent.GetComponent<PlayerSoundController>();
    }

/*    private void Update()
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
    }*/

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("EnemyAttack"))
        {
            if (!activeColliders.Contains(collider))
            {
                activeColliders.Add(collider);
                colliderDistanceMap[collider] = maxAttackDistance;
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
            if (activeColliders.Contains(collider) && !collider.GetComponent<EnemyAttackInfo>().isAttacked
                && collider.GetComponent<EnemyAttackInfo>().grazable && !collider.GetComponent<EnemyAttackInfo>().grazed)
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
            GetComponentInChildren<GrazeOutlineController>().AppearAndFadeOut();
            Debug.Log($"Gauge Increased by {PlayerCharacter.Instance.stat.grazeGainOnGraze.GetValue() / distance} with ratio {distance}");
            collider.GetComponent<EnemyAttackInfo>().grazed = true;
            _playerSoundController.Trigger(PlayerSoundType.GRAZE);
            GetComponentInChildren<EventObject>().Trigger();
        }
    }

    private void RemoveCollider(Collider2D collider)
    {
        activeColliders.Remove(collider);
        colliderDistanceMap.Remove(collider);
    }
}
