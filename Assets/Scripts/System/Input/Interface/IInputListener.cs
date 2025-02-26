namespace System.Input.Interface
{
    public enum InputType
    {
        Down, Up, Press
    }

    public interface IInputListener
    {
        public void OnKey(ActionCode action, InputType type);
    }
}