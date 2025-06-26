using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector
{
    public class SimplePlayerDetector : MonoBehaviour, IDetector<PlayerCharacter>
    {
        private SimplePlayerDetector() { }

        public static SimplePlayerDetector Create(global::GyeMong.GameSystem.Creature.Creature creature)
        {
            SimplePlayerDetector detector = creature.gameObject.AddComponent<SimplePlayerDetector>();
            detector.creature = creature;
            return detector;
        }
    
        private global::GyeMong.GameSystem.Creature.Creature creature;
    
        public List<PlayerCharacter> DetectTargets()
        {
            return new List<PlayerCharacter> { DetectTarget() };
        }

        public PlayerCharacter DetectTarget()
        {
            return SceneContext.Character;
        }
    }
}
