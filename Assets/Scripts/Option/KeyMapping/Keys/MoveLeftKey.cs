using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeftKey : KeyMapping
{
    protected override ActionCode ActionCode => ActionCode.MoveLeft;
    protected override string InitialCode => "LeftArrow";
}
