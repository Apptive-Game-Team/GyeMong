using System;
using System.Collections.Generic;
using Creature.Attack;
using playerCharacter;
using UnityEngine;

using LastEnemyAttackInfo = Creature.Boss.EnemyAttackInfo; 

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
        if (OnExitWithLast(collider)) return;
        
        if (collider.CompareTag("EnemyAttack"))
        {
            if (activeColliders.Contains(collider) && !collider.GetComponent<AttackObjectController>().isAttacked
                && collider.GetComponent<AttackObjectController>().AttackInfo.grazable && !collider.GetComponent<AttackObjectController>().isGrazed)
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

    [Obsolete("Don't use this method")]
    private bool OnExitWithLast(Collider2D collider)
    {
        try
        {
            if (collider.CompareTag("EnemyAttack"))
            {
                if (activeColliders.Contains(collider) && !collider.GetComponent<LastEnemyAttackInfo>().isAttacked
                                                       && collider.GetComponent<LastEnemyAttackInfo>().grazable &&
                                                       !collider.GetComponent<LastEnemyAttackInfo>().grazed)
                {
                    GrazedWithLast(collider);
                    RemoveCollider(collider);
                }
                else
                {
                    RemoveCollider(collider);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    [Obsolete("Don't use this method")]
    private void GrazedWithLast(Collider2D collider)
    {
        if (colliderDistanceMap.TryGetValue(collider, out float distance))
        {
            PlayerCharacter.Instance.GrazeIncreaseGauge(distance);
            GetComponentInChildren<GrazeOutlineController>().AppearAndFadeOut();
            Debug.Log($"Gauge Increased by {PlayerCharacter.Instance.stat.GrazeGainOnGraze / distance} with ratio {distance}");
            collider.GetComponent<LastEnemyAttackInfo>().grazed = true;
            _playerSoundController.Trigger(PlayerSoundType.GRAZE);
            GetComponentInChildren<EventObject>().Trigger();
        }
    }
    
    private void Grazed(Collider2D collider)
    {
        if (colliderDistanceMap.TryGetValue(collider, out float distance))
        {
            PlayerCharacter.Instance.GrazeIncreaseGauge(distance);
            GetComponentInChildren<GrazeOutlineController>().AppearAndFadeOut();
            Debug.Log($"Gauge Increased by {PlayerCharacter.Instance.stat.GrazeGainOnGraze / distance} with ratio {distance}");
            collider.GetComponent<AttackObjectController>().isGrazed = true;
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
