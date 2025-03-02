using UnityEngine;
using Creature.Player.Component;

    [CreateAssetMenu(fileName = "StatData",menuName ="ScriptableObject/StatData")]
    public class StatData : ScriptableObject
    {
        public StatComponent stat;
    }
    
