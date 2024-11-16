using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseKey : KeyMapping
{
    protected override ActionCode ActionCode => ActionCode.Defend;
    protected override string InitialCode => "S";
}
