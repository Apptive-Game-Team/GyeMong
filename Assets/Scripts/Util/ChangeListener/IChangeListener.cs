namespace Util.Interface
{
    public interface IChangeListener<T>
    {
        void OnChanged(T data);
    }
}