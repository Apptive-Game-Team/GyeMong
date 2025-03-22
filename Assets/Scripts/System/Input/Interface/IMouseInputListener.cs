using System.Game.Rune.RuneUI;

namespace System.Input.Interface
{
    public interface IMouseInputListener
    {
        public void OnMouseInput(MouseInputState state, ISelectableUI ui);   
    }
}