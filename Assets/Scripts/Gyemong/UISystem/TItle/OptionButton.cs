using Gyemong.UISystem.Option;
using UnityEngine;

namespace Gyemong.UISystem.TItle
{
    public class OptionButton : MonoBehaviour
    {
        public void Option()
        {
            OptionUIToggler.Instance.ToggleOption();
        }
    }
}
