using System.Collections;
using System.Collections.Generic;
using System.UI.Option;
using UnityEngine;

public class ResumeButton : MonoBehaviour
{
    public void OnclickButton()
    {
        OptionUIToggler.Instance.ToggleOption();
    }
}
