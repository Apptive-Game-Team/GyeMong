using System.Collections.Generic;
using GyeMong.GameSystem.Creature.Player;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector
{
    public class SimplePlayerDistanceDetector : MonoBehaviour, IDetector<PlayerCharacter>
    {
        private SimplePlayerDistanceDetector() { }

        public static SimplePlayerDistanceDetector Create(Mob mob)
        {
            SimplePlayerDistanceDetector detector = mob.gameObject.AddComponent<SimplePlayerDistanceDetector>();
            detector._mob = mob;
            return detector;
        }
    
        private Mob _mob;
    
        public List<PlayerCharacter> DetectTargets()
        {
            return new List<PlayerCharacter> { DetectTarget() };
        }

        public PlayerCharacter DetectTarget()
        {
            PlayerCharacter player = PlayerCharacter.Instance;
        
            if (Vector3.Distance(player.transform.position, transform.position) < _mob.DetectionRange)
            {
                return player;
            }
            return null;
        }
    }
}
