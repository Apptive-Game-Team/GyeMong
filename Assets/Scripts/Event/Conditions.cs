using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public abstract class Condition
{
    public abstract bool Check();
}

[Serializable]
public class BoolCondition : Condition
{
    [SerializeReference]
    private bool condition = true;

    public override bool Check()
    {
        return condition;
    }
}

[Serializable]
public class NotCondition : Condition
{
    [SerializeReference]
    private Condition condition;

    public override bool Check()
    {
        return !condition.Check();
    }
}
