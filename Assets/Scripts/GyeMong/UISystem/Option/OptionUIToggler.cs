using GyeMong.InputSystem;

namespace GyeMong.UISystem.Option
{
    public class OptionUIToggler : UIWindowToggler<OptionUIToggler>
    {
        protected override void Awake()
        {
            base.Awake();
            toggleKeyActionCode = ActionCode.Option;
        }
        
        public void ToggleOption()
        {
            OpenOrCloseOption();
        }
    }
}
