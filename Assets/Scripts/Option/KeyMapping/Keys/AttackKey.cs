using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackKey : KeyMapping
{
    protected override ActionCode ActionCode => ActionCode.Attack;
    protected override string InitialCode => "A";
}
