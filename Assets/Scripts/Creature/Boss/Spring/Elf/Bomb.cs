using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Spring.Elf
{
    public class Bomb : MonoBehaviour
    {
        private float damage = 20f;
        private EnemyAttackInfo enemyAttackInfo;

        private SoundObject _soundObject;
        private SoundObject _explosionSoundObject;
        private EventObject _eventObject;

        private void Awake()
        {
            _eventObject = GetComponent<EventObject>();
            _explosionSoundObject = GetComponent<SoundObject>();
            
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, _soundObject, true, true);
        }

        private void OnEnable()
        {
            StartCoroutine(WaitWarning());
        }
        private IEnumerator WaitWarning()
        {
            yield return new WaitForSeconds(2f);
            Explode();
        }

        private void Explode()
        {
            _explosionSoundObject.PlayAsync();
            _eventObject.Trigger();
            float explosionRadius = 2f;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy.CompareTag("Player"))
                {
                    PlayerCharacter.Instance.TakeDamage(damage / 2);
                }
            }
            Destroy(gameObject);
        }
    }
}
