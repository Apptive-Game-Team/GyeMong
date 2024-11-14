using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionKeyMapping : KeyMapping
{
    protected override ActionCode ActionCode => ActionCode.Interaction;
    protected override string InitialCode => "Z";
}


