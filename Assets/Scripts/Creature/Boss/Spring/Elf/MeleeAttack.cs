using playerCharacter;
using System.Collections;
using System.Collections.Generic;
using Rework;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    private float damage = 30f;
    private EnemyAttackInfo enemyAttackInfo;

    private void Awake()
    {
        enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
        enemyAttackInfo.Initialize(damage, null, false, false);
    }
}
