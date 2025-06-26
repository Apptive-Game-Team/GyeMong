using System.Collections.Generic;
using GyeMong.EventSystem;
using GyeMong.GameSystem.Creature.Attack;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Player.Controller
{
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
    
        private void Grazed(Collider2D collider)
        {
            if (colliderDistanceMap.TryGetValue(collider, out float distance))
            {
                SceneContext.Character.GrazeIncreaseGauge(distance);
                GetComponentInChildren<GrazeOutlineController>().AppearAndFadeOut();
                Debug.Log($"Gauge Increased by {SceneContext.Character.stat.GrazeGainOnGraze / distance} with ratio {distance}");
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
}
