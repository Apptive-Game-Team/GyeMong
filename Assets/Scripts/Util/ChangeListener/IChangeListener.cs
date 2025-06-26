namespace Util.ChangeListener
{
    public interface IChangeListener<T>
    {
        void OnChanged(T data);
    }
}