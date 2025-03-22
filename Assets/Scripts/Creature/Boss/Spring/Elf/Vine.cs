using System;
using System.Collections;
using UnityEngine;

namespace Creature.Boss.Spring.Elf
{
    [Obsolete("Use AttackObjectController instead")]
    public class Vine : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(ActivateVine());
        }
        private IEnumerator ActivateVine()
        {
            yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}