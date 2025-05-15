using System.Collections.Generic;
using UnityEngine;

namespace GyeMong.GameSystem.Creature.Attack.Component.Sound
{
    [CreateAssetMenu(fileName = "AttackObjectSounds", menuName = "Creature/Attack/AttackObjectSounds")]
    public class AttackObjectSounds : ScriptableObject
    {
        public List<string> startSoundId;
        public List<string> endSoundId;
        public List<string> hitSoundId;
    }
}
