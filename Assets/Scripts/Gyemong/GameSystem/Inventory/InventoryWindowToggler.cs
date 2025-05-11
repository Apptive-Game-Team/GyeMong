using Gyemong.InputSystem;
using Gyemong.UISystem;

namespace Gyemong.GameSystem.Inventory
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
