using System;
using System.Collections;
using UnityEngine;

namespace Gyemong.GameSystem.Creature.Mob.StateMachineMob.Boss.Spring.Golem
{
    [Obsolete("Use AttackObjectController instead")]
    public class Root : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(OffRootObjects());
        }
        private IEnumerator OffRootObjects()
        {
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
    }
}