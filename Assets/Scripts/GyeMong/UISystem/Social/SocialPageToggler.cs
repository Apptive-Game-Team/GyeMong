using System.Collections;
using System.Collections.Generic;
using GyeMong.InputSystem;
using GyeMong.UISystem;
using Unity.VisualScripting;
using UnityEngine;

public class SocialPageToggler : UIWindowToggler<SocialPageToggler>
{

    protected override void Awake()
    {
        base.Awake();
        toggleKeyActionCode = ActionCode.Interaction;
    }
    
    public void OnClick()
    {
        OpenOrCloseOption();
    }
}
