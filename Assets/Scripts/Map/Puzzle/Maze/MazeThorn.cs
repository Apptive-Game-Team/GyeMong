using System;
using System.Collections;
using playerCharacter;
using UnityEngine;

namespace Map.Puzzle.Maze
{
    [Obsolete("This class is no longer used. Will be removed in the future.")]
    public class MazeThorn : MonoBehaviour
    {
        private float damage = 10f;
        private float damageDelay = 1f;
        private EnemyAttackInfo enemyAttackInfo;

        private void Awake() 
        {
            enemyAttackInfo = gameObject.AddComponent<EnemyAttackInfo>();
            enemyAttackInfo.Initialize(damage, null, false, false,true, damageDelay); 
        }
    }
}
