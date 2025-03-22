using System.Collections;
using UnityEngine;

namespace Creature.Mob.StateMachineMob.Minion.ShadowOfHero
{
    public class SwordAura : MonoBehaviour
    {
        private static bool toggleYScale = false;
    
        public static IEnumerator Create(Transform transform, Vector3 direction, GameObject prefab)
        {
            GameObject swordAura = Instantiate(prefab, transform.position + direction * 0.5f, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg), transform);
            // toggle y scale
            Vector3 scale = swordAura.transform.localScale;
            swordAura.transform.localScale = new Vector3(scale.x, scale.y * (toggleYScale ? 1 : -1) , scale.z);
            toggleYScale = !toggleYScale;
            return swordAura.GetComponent<SwordAura>().AttackRoutine();
        }

        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(0.04f * 2);
            GetComponent<BoxCollider2D>().enabled = true;
            yield return new WaitForSeconds(0.04f * 2);
            GetComponent<BoxCollider2D>().enabled = false;
            yield return new WaitForSeconds(0.04f * 2);
            Destroy(gameObject);
        }
    }
}
