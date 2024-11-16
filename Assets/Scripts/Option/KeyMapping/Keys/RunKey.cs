using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunKey : KeyMapping
{
    protected override ActionCode ActionCode => ActionCode.Run;
    protected override string InitialCode => "LeftShift";
}
