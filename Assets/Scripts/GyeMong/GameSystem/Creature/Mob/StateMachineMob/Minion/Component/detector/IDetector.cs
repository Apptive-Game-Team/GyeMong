using System.Collections.Generic;

namespace GyeMong.GameSystem.Creature.Mob.StateMachineMob.Minion.Component.detector
{
    public interface IDetector<T>
    {
        public List<T> DetectTargets();
        public T DetectTarget();
    }
}
