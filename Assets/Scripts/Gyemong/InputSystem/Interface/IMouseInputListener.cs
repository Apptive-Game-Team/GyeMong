namespace Gyemong.InputSystem.Interface
{
    public interface IMouseInputListener
    {
        public void OnMouseInput(MouseInputState state, ISelectableUI ui);   
    }
}