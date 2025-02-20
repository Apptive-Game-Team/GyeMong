using System;
using System.Collections;
using System.Collections.Generic;
using playerCharacter;
using Unity.VisualScripting;
using UnityEngine;

public class GrazeController : MonoBehaviour
{
    [SerializeField] private float maxAttackDistance = 1f;
    private List<Collider2D> activeColliders = new();
    private Dictionary<Collider2D, float> colliderDistanceMap = new();
    private PlayerSoundController _playerSoundController;
    private PlayerCharacter player;

    private void Start()
    {
        _playerSoundController = transform.parent.GetComponent<PlayerSoundController>();
        player = PlayerCharacter.Instance;
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
        if (collider.CompareTag("EnemyAttack"))
        {
            EnemyAttackInfo enemyAttackInfo = collider.GetComponent<EnemyAttackInfo>();
            if (activeColliders.Contains(collider) && !enemyAttackInfo.isAttacked
                && enemyAttackInfo.grazable && !enemyAttackInfo.grazed)
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
            GrazeIncreaseGauge(distance);
            GetComponentInChildren<GrazeOutlineController>().AppearAndFadeOut();
            Debug.Log($"Gauge Increased by {PlayerCharacter.Instance.GaugeIncreaseValue / distance} with ratio {distance}");
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

    private void GrazeIncreaseGauge(float ratio)
    {
        player.SoundController.Trigger(PlayerSoundType.GRAZE);
        player.CurSkillGauge += player.GaugeIncreaseValue / ratio;
        if (player.CurSkillGauge > player.MaxSkillGauge)
        {
            player.CurSkillGauge = player.MaxSkillGauge;
        }
        player.changeListenerCaller.CallSkillGaugeChangeListeners(player.CurSkillGauge);
    }
}
