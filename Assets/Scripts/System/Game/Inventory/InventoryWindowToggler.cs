using System.Input;
using System.UI;

namespace System.Game.Inventory
{
    public class InventoryWindowToggler : UIWindowToggler<InventoryWindowToggler>
    {
        protected override void Awake()
        {
            base.Awake();
            toggleKeyActionCode = ActionCode.Menu;
        }
    }
}
