using Gyemong.InputSystem;
using Gyemong.UISystem;

namespace Gyemong.GameSystem.Rune.RuneUI
{
    public class RuneWindowToggler : UIWindowToggler<RuneWindowToggler>
    {
        protected override void Awake()
        {
            base.Awake();
            toggleKeyActionCode = ActionCode.RunePage;
        }
    }
}
