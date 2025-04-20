using System.Input;
using System.UI;

namespace System.Game.Inventory
{
    public class InveentoryWindowToggler : UIWindowToggler<InveentoryWindowToggler>
    {
        protected override void Awake()
        {
            base.Awake();
            toggleKeyActionCode = ActionCode.RunePage;
        }
    }
}
