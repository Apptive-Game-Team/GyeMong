using System;
using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creature.Boss.Spring.Golem
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