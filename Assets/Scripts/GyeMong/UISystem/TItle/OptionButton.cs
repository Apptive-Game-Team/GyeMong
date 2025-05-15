using GyeMong.UISystem.Option;
using UnityEngine;

namespace GyeMong.UISystem.TItle
{
    public class OptionButton : MonoBehaviour
    {
        public void Option()
        {
            OptionUIToggler.Instance.ToggleOption();
        }
    }
}
