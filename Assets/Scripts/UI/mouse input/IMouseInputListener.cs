namespace UI.mouse_input
{
    public interface IMouseInputListener
    {
        public void OnMouseInput(MouseInputState state, SelectableUI ui);   
    }
}