using System.Collections;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Minion.ShadowOfHero
{
    public class SkillSwordAura : MonoBehaviour
    {
        private Vector3 _moveDirection;
        private float _speed;
        private float _duration;
        
        public static IEnumerator Create(Transform transform, Vector3 direction, GameObject prefab, float speed = 10f, float duration = 1f)
        {
            try
            {
                GameObject swordAura = Instantiate(prefab, transform.position + direction * 0.5f, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), transform);
                swordAura.GetComponent<SkillSwordAura>()._moveDirection = direction;
                swordAura.GetComponent<SkillSwordAura>()._duration = duration;
                swordAura.GetComponent<SkillSwordAura>()._speed = speed;
                return swordAura.GetComponent<SkillSwordAura>().AttackRoutine();
            } catch (MissingReferenceException e)
            {
                Debug.Log("SkillSwordAura.Create() failed: " + e.Message);
                return null;
            }
        }

        private IEnumerator AttackRoutine()
        {
            
            float elapsedTime = 0;
            while (elapsedTime < _duration)
            {
                elapsedTime += Time.deltaTime;
                try
                {
                    transform.position += _moveDirection * _speed * Time.deltaTime;
                } catch (MissingReferenceException e)
                {
                    Debug.Log("SkillSwordAura.AttackRoutine() failed: " + e.Message);
                    yield break;
                }
                
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}