using System.Collections.Generic;

namespace Creature.Mob.StateMachineMob.Minion.Component.detector
{
    public interface IDetector<T>
    {
        public List<T> DetectTargets();
        public T DetectTarget();
    }
}
