namespace System.Input.Interface
{
    public enum InputType
    {
        Down, Up, Press
    }
    [Obsolete("Use InputManager.OnKeyEvent instead")]
    public interface IInputListener
    {
        public void OnKey(ActionCode action, InputType type);
    }
}