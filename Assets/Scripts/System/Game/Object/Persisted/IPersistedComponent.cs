namespace System.Game.Object.Persisted
{
    [Serializable]
    public abstract class ComponentData {}
    public interface IPersistedComponent
    {
        public ComponentData Save();
        public void Load(ComponentData data);
    }
}
