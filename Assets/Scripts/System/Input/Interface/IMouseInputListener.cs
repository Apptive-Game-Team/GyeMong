using UI.mouse_input;

namespace System.Input.Interface
{
    public interface IMouseInputListener
    {
        public void OnMouseInput(MouseInputState state, ISelectableUI ui);   
    }
}