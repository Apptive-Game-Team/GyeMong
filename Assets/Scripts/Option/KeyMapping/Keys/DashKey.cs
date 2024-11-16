using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashKey : KeyMapping
{
    protected override ActionCode ActionCode => ActionCode.Dash;
    protected override string InitialCode => "X";
}
