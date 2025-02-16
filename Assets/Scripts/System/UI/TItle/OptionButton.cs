using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public void Option()
    {
        OptionUI.Instance.OpenOrCloseOption();
    }
}
