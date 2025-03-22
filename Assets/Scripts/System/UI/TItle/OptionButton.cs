using System.UI.Option;
using UnityEngine;

namespace System.UI.TItle
{
    public class OptionButton : MonoBehaviour
    {
        public void Option()
        {
            OptionUIToggler.Instance.ToggleOption();
        }
    }
}
