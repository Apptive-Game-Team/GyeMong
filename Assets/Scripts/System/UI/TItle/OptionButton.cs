using System.Collections;
using System.Collections.Generic;
using System.UI.Option;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public void Option()
    {
        OptionUI.Instance.OpenOrCloseOption();
    }
}
