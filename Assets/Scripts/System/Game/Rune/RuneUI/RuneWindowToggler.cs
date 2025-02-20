using System.UI;

namespace System.Game.Rune.RuneUI
{
    public class RuneWindowToggler : UIWindowToggler<RuneWindowToggler>
    {
        protected override void Awake()
        {
            base.Awake();
            toggleKeyActionCode = ActionCode.RunePage;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
    }
}
