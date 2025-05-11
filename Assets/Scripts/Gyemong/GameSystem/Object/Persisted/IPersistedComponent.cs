using System;

namespace Gyemong.GameSystem.Object.Persisted
{
    [Serializable]
    public abstract class ComponentData {}
    public interface IPersistedComponent
    {
        public ComponentData Save();
        public void Load(ComponentData data);
    }
}
